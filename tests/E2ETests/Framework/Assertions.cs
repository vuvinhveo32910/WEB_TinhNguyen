using System;
using System.Text.RegularExpressions;

namespace WEB_TinhNguyen.Tests.E2E.Framework
{
    public class AssertException : Exception
    {
        public AssertException(string message) : base(message) { }
    }

    public static class Assert
    {
        public static void True(bool condition, string message)
        {
            if (!condition)
            {
                throw new AssertException($"Assert.True Failed: {message}");
            }
        }

        public static void False(bool condition, string message)
        {
            if (condition)
            {
                throw new AssertException($"Assert.False Failed: {message}");
            }
        }

        public static void Equal<T>(T expected, T actual, string message)
        {
            if (!object.Equals(expected, actual))
            {
                throw new AssertException($"Assert.Equal Failed: Expected '{expected}', Got '{actual}'. {message}");
            }
        }

        public static void NotEqual<T>(T notExpected, T actual, string message)
        {
            if (object.Equals(notExpected, actual))
            {
                throw new AssertException($"Assert.NotEqual Failed: Value should not equal '{notExpected}'. {message}");
            }
        }

        public static void NotNull(object? obj, string message)
        {
            if (obj == null)
            {
                throw new AssertException($"Assert.NotNull Failed: Object was null. {message}");
            }
        }

        public static void Null(object? obj, string message)
        {
            if (obj != null)
            {
                throw new AssertException($"Assert.Null Failed: Expected null, but got object. {message}");
            }
        }

        public static void Contains(string expectedSubstring, string actualText, string message)
        {
            if (string.IsNullOrEmpty(actualText) || !actualText.Contains(expectedSubstring, StringComparison.OrdinalIgnoreCase))
            {
                throw new AssertException($"Assert.Contains Failed: Text does not contain '{expectedSubstring}'. {message}");
            }
        }

        public static void Matches(string regexPattern, string actualText, string message)
        {
            if (string.IsNullOrEmpty(actualText) || !Regex.IsMatch(actualText, regexPattern))
            {
                throw new AssertException($"Assert.Matches Failed: '{actualText}' does not match pattern '{regexPattern}'. {message}");
            }
        }

        public static void InRange(decimal actual, decimal min, decimal max, string message)
        {
            if (actual < min || actual > max)
            {
                throw new AssertException($"Assert.InRange Failed: Value {actual} outside range [{min}, {max}]. {message}");
            }
        }

        public static void GreaterThan(decimal actual, decimal threshold, string message)
        {
            if (actual <= threshold)
            {
                throw new AssertException($"Assert.GreaterThan Failed: Value {actual} is not greater than {threshold}. {message}");
            }
        }

        public static void RequireFile(string fullPath, string description)
        {
            if (!System.IO.File.Exists(fullPath))
            {
                throw new AssertException($"[SKIP] Pending implementation: {description} ({System.IO.Path.GetFileName(fullPath)})");
            }
        }

        public static void RequireDirectory(string fullPath, string description)
        {
            if (!System.IO.Directory.Exists(fullPath))
            {
                throw new AssertException($"[SKIP] Pending implementation: {description} ({System.IO.Path.GetFileName(fullPath)})");
            }
        }

        public static Type RequireType(string fullTypeName, string description)
        {
            var type = E2ETestContext.FindType(fullTypeName);
            if (type == null)
            {
                throw new AssertException($"[SKIP] Pending implementation: {description} ({fullTypeName})");
            }
            return type;
        }

        public static TException Throws<TException>(Action action, string message) where TException : Exception
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (ex is TException tex)
                {
                    return tex;
                }
                throw new AssertException($"Assert.Throws Failed: Expected exception {typeof(TException).Name}, but got {ex.GetType().Name}. {message}");
            }
            throw new AssertException($"Assert.Throws Failed: Expected exception {typeof(TException).Name}, but no exception was thrown. {message}");
        }

        public static T IsType<T>(object? obj, string message)
        {
            if (obj == null)
            {
                throw new AssertException($"Assert.IsType Failed: Expected type {typeof(T).Name}, but object was null. {message}");
            }
            if (obj is not T typed)
            {
                throw new AssertException($"Assert.IsType Failed: Expected type {typeof(T).Name}, but got {obj.GetType().Name}. {message}");
            }
            return typed;
        }

        public static void Same(object? expected, object? actual, string message)
        {
            if (!object.ReferenceEquals(expected, actual))
            {
                throw new AssertException($"Assert.Same Failed: Objects are not reference equal. {message}");
            }
        }
    }
}
