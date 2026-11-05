# Dispatch Task for Reviewer M1.1

## Objective
Independently review Milestone 1 (Area & Data Foundation) implementation.
Verify correctness, completeness, robustness, and interface conformance per ORIGINAL_REQUEST.md and PROJECT.md.
Run the build command `XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx` and verify results.

## Files to Review
- `WEB_TinhNguyen/Program.cs`
- `WEB_TinhNguyen/Data/ApplicationDbContext.cs`
- `tailwind.config.js`
- `WEB_TinhNguyen/Areas/CuaHang/Models/*.cs` (7 models)
- `WEB_TinhNguyen/Areas/CuaHang/Helpers/*.cs` (2 helpers)
- `WEB_TinhNguyen/Areas/CuaHang/Views/_ViewImports.cshtml`, `_ViewStart.cshtml`
- Worker handoff: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_m1/handoff.md`

## Verdict
Explicitly issue a verdict in your handoff report: `APPROVE` or `REQUEST_CHANGES`.
Write report to `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_m1_1/handoff.md`.
Send message to orchestrator (Recipient: 108b9d81-1dd6-495f-84eb-99dbb197aebe) when done.

## 2026-11-03T12:59:52Z
You are teamwork_reviewer_m1_1.
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_m1_1
Your parent is the Project Orchestrator (Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe).

MANDATORY INSTRUCTIONS:
1. First read the original user request at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
2. Read the master project document at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md
3. Read your specific dispatch instructions at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_m1_1/DISPATCH.md
4. Review Milestone 1 files and verify compilation with `XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx`.
5. Write your handoff report to /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_m1_1/handoff.md and state your verdict (APPROVE or REQUEST_CHANGES).
6. Send a message to the orchestrator (Recipient: 108b9d81-1dd6-495f-84eb-99dbb197aebe) when done.
