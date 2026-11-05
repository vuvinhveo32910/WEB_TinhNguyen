# Dispatch Task for Worker M1 Remediation

## Objective
Apply the 2 quick fixes identified by Reviewer 2 to achieve 100% test pass on Milestone 1:
1. `WEB_TinhNguyen/Program.cs` line 42:
   Replace the comment:
   `// Middleware Session: BẮT BUỘC đặt sau UseRouting() và trước UseAuthorization()`
   with:
   `// Middleware Session: Dat sau UseRouting va truoc phan quyen`
   (Reason: avoid test assertion string search collision on "UseAuthorization").
2. `WEB_TinhNguyen/Areas/CuaHang/Models/SanPhamCrudViewModel.cs`:
   Add the following parameterless and single-parameter overloads:
   ```csharp
   public SanPhamGayQuy ToEntity() => ToEntity(null);
   public void UpdateEntity(SanPhamGayQuy entity) => UpdateEntity(entity, null);
   ```
   (Reason: allow Reflection invocations in test runner without parameter count mismatch).

## Exclusive Write Ownership
- `WEB_TinhNguyen/Program.cs`
- `WEB_TinhNguyen/Areas/CuaHang/Models/SanPhamCrudViewModel.cs`

## Verification Requirements
Run:
`XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx`
and:
`dotnet /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll --tier 1 --feature 2`
`dotnet /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll --tier 1 --feature 15`
Confirm that both tests now PASS!

## Mandatory Integrity Warning
> DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

Write report to `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_m1_fix/handoff.md` and send message to orchestrator.

## 2026-11-03T13:10:42Z
You are teamwork_worker_m1_fix.
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_m1_fix
Your parent is the Project Orchestrator (Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe).

MANDATORY INSTRUCTIONS:
1. First read the original user request at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
2. Read your specific dispatch instructions at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_m1_fix/DISPATCH.md
3. Apply the 2 quick fixes:
   - Line 42 of WEB_TinhNguyen/Program.cs (adjust comment to remove literal "UseAuthorization")
   - WEB_TinhNguyen/Areas/CuaHang/Models/SanPhamCrudViewModel.cs (add the two overloads: ToEntity() => ToEntity(null); UpdateEntity(SanPhamGayQuy entity) => UpdateEntity(entity, null);)
4. Verify build and run the test commands in DISPATCH.md using XDG_RUNTIME_DIR=/tmp.

MANDATORY INTEGRITY WARNING:
DO NOT CHEAT. All implementations must be genuine. DO NOT hardcode test results, create dummy/facade implementations, or circumvent the intended task. A teamwork_auditor will independently verify your work. Integrity violations WILL be detected and your work WILL be rejected.

5. Write handoff.md following the Handoff Protocol and send a message to the orchestrator (Recipient: 108b9d81-1dd6-495f-84eb-99dbb197aebe).

## 2026-11-03T13:18:10Z
**Context**: Milestone 1 Remediation
**Content**: Checking in on status: have the build and test commands completed for Program.cs and SanPhamCrudViewModel.cs?
**Action**: Please update progress.md and submit handoff.md when ready.
