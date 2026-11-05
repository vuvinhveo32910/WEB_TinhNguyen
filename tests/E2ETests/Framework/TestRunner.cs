using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace WEB_TinhNguyen.Tests.E2E.Framework
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class E2ETestAttribute : Attribute
    {
        public int Tier { get; }
        public int Feature { get; set; }
        public string Name { get; }
        public string Description { get; set; } = string.Empty;

        public E2ETestAttribute(int tier, string name)
        {
            Tier = tier;
            Name = name;
        }

        public E2ETestAttribute(int tier, int feature, string name)
        {
            Tier = tier;
            Feature = feature;
            Name = name;
        }
    }

    public enum TestStatus
    {
        Passed,
        Failed,
        Skipped
    }

    public class TestResult
    {
        public string Name { get; set; } = string.Empty;
        public int Tier { get; set; }
        public int Feature { get; set; }
        public TestStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public string? StackTrace { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public static class TestRunnerEngine
    {
        public static int Run(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int targetTier = 0; // 0 = all
            int targetFeature = 0;
            string filter = string.Empty;
            bool verbose = false;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--tier" && i + 1 < args.Length && int.TryParse(args[i + 1], out var t))
                {
                    targetTier = t;
                    i++;
                }
                else if (args[i] == "--feature" && i + 1 < args.Length && int.TryParse(args[i + 1], out var f))
                {
                    targetFeature = f;
                    i++;
                }
                else if (args[i] == "--filter" && i + 1 < args.Length)
                {
                    filter = args[i + 1];
                    i++;
                }
                else if (args[i] == "--verbose" || args[i] == "-v")
                {
                    verbose = true;
                }
            }

            Console.WriteLine("================================================================================");
            Console.WriteLine("    WEB_TinhNguyen Volunteer Fundraising E2E Test Suite (Tiers 1-4)");
            Console.WriteLine("================================================================================");
            Console.WriteLine($"Configuration: Tier Filter=[{(targetTier == 0 ? "ALL" : targetTier.ToString())}], Feature=[{(targetFeature == 0 ? "ALL" : targetFeature.ToString())}], Keyword=[{(string.IsNullOrEmpty(filter) ? "NONE" : filter)}]");
            Console.WriteLine();

            var currentAssembly = Assembly.GetExecutingAssembly();
            var testClasses = currentAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Namespace != null && t.Namespace.Contains("Tests.E2E"))
                .ToList();

            var results = new List<TestResult>();
            var stopwatch = Stopwatch.StartNew();

            foreach (var testClass in testClasses)
            {
                var methods = testClass.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                    .Where(m => m.GetCustomAttribute<E2ETestAttribute>() != null)
                    .ToList();

                if (!methods.Any()) continue;

                object? instance = null;

                foreach (var method in methods)
                {
                    var attr = method.GetCustomAttribute<E2ETestAttribute>()!;

                    if (targetTier != 0 && attr.Tier != targetTier) continue;
                    if (targetFeature != 0 && attr.Feature != targetFeature) continue;
                    if (!string.IsNullOrEmpty(filter) && !attr.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)
                        && !method.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (!method.IsStatic && instance == null)
                    {
                        try
                        {
                            instance = Activator.CreateInstance(testClass);
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"[ERROR] Could not instantiate {testClass.Name}: {ex.Message}");
                            Console.ResetColor();
                            break;
                        }
                    }

                    var testResult = new TestResult
                    {
                        Name = attr.Name,
                        Tier = attr.Tier,
                        Feature = attr.Feature
                    };

                    var methodStopwatch = Stopwatch.StartNew();
                    try
                    {
                        method.Invoke(method.IsStatic ? null : instance, null);
                        methodStopwatch.Stop();
                        testResult.Status = TestStatus.Passed;
                        testResult.Duration = methodStopwatch.Elapsed;

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("  [PASS] ");
                        Console.ResetColor();
                        Console.WriteLine($"Tier {attr.Tier} | F{attr.Feature:D2} | {attr.Name} ({testResult.Duration.TotalMilliseconds:F1}ms)");
                    }
                    catch (TargetInvocationException tie)
                    {
                        methodStopwatch.Stop();
                        testResult.Duration = methodStopwatch.Elapsed;
                        var inner = tie.InnerException ?? tie;

                        if (inner.Message.StartsWith("[SKIP]") || inner.Message.Contains("pending implementation"))
                        {
                            testResult.Status = TestStatus.Skipped;
                            testResult.ErrorMessage = inner.Message;

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("  [SKIP] ");
                            Console.ResetColor();
                            Console.WriteLine($"Tier {attr.Tier} | F{attr.Feature:D2} | {attr.Name} -> {inner.Message}");
                        }
                        else
                        {
                            testResult.Status = TestStatus.Failed;
                            testResult.ErrorMessage = inner.Message;
                            testResult.StackTrace = inner.StackTrace;

                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("  [FAIL] ");
                            Console.ResetColor();
                            Console.WriteLine($"Tier {attr.Tier} | F{attr.Feature:D2} | {attr.Name}");
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine($"         Reason: {inner.Message}");
                            if (verbose && !string.IsNullOrEmpty(inner.StackTrace))
                            {
                                Console.WriteLine($"         Stack: {inner.StackTrace.Trim()}");
                            }
                            Console.ResetColor();
                        }
                    }
                    catch (Exception ex)
                    {
                        methodStopwatch.Stop();
                        testResult.Status = TestStatus.Failed;
                        testResult.Duration = methodStopwatch.Elapsed;
                        testResult.ErrorMessage = ex.Message;
                        testResult.StackTrace = ex.StackTrace;

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("  [FAIL] ");
                        Console.ResetColor();
                        Console.WriteLine($"Tier {attr.Tier} | F{attr.Feature:D2} | {attr.Name} ({ex.Message})");
                    }

                    results.Add(testResult);
                }
            }

            stopwatch.Stop();

            Console.WriteLine();
            Console.WriteLine("================================================================================");
            Console.WriteLine("                              TEST EXECUTION SUMMARY");
            Console.WriteLine("================================================================================");

            var passed = results.Count(r => r.Status == TestStatus.Passed);
            var failed = results.Count(r => r.Status == TestStatus.Failed);
            var skipped = results.Count(r => r.Status == TestStatus.Skipped);
            var total = results.Count;

            for (int t = 1; t <= 5; t++)
            {
                var tierResults = results.Where(r => r.Tier == t).ToList();
                if (tierResults.Any())
                {
                    var tPass = tierResults.Count(r => r.Status == TestStatus.Passed);
                    var tFail = tierResults.Count(r => r.Status == TestStatus.Failed);
                    var tSkip = tierResults.Count(r => r.Status == TestStatus.Skipped);
                    Console.WriteLine($"  Tier {t}: Total: {tierResults.Count,3} | Passed: {tPass,3} | Failed: {tFail,3} | Skipped: {tSkip,3}");
                }
            }

            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.ForegroundColor = failed > 0 ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine($"  OVERALL: {total} Tests | {passed} Passed | {failed} Failed | {skipped} Skipped in {stopwatch.Elapsed.TotalSeconds:F2}s");
            Console.ResetColor();
            Console.WriteLine("================================================================================");

            return failed > 0 ? 1 : 0;
        }
    }
}
