# Task Dispatch: Forensic Auditor R2_1 - Integrity Forensics

## Role
Teamwork Forensic Auditor (`teamwork_auditor`)

## Working Directory
`/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_auditor_r2_1`

## Mandatory Reference Documents
- Authoritative User Request: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- Master Project Plan: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator_r2/PROJECT.md`

## Audit Mission
Perform independent forensic verification of the codebase:
1. Check for genuine implementation vs dummy facades or hardcoded values.
2. Check that no cheating patterns exist: hardcoded test results, spoofed assertions, mock bypasses.
3. Verify that `Areas/CuaHang` contains genuine controllers (`AdminDonHangController`, `AdminSanPhamController`, `CuaHangController`, `GioHangController`), real database entities, models, views, and actual logic for charity fundraising and order processing.
4. Verify solution build:
   `export XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp HOME=/tmp; dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj --configfile tests/NuGet.Config --verbosity minimal`
5. Verify test execution:
   `bash tests/run_e2e_tests.sh --all`

## Required Verdict
In your `handoff.md`, state clearly:
`Verdict: CLEAN` or `Verdict: INTEGRITY VIOLATION (evidence)`
Send a completion message with your verdict.


## 2026-11-04T01:16:05Z
[Message] timestamp=2026-11-04T01:16:05Z sender=539c754b-e018-4f42-a411-9b35533a184a priority=MESSAGE_PRIORITY_HIGH content=You are teamwork_auditor_r2_1.
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_auditor_r2_1
Authoritative user request record: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
Your task dispatch instructions are in: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_auditor_r2_1/DISPATCH.md

You MUST read /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md before starting work.

Your objective:
Conduct an independent forensic integrity audit of the entire solution, verifying that all implementations in `Areas/CuaHang/` (Controllers, Models, Views, Helpers) and Layouts are genuine, authentic, and without shortcuts, facade mocks, or hardcoded cheating.
Run the build and tests:
`export XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp HOME=/tmp; dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj --configfile tests/NuGet.Config --verbosity minimal`
`bash tests/run_e2e_tests.sh --all`

Write your forensic verdict (CLEAN / INTEGRITY VIOLATION) in:
`/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_auditor_r2_1/handoff.md`.
Send a completion message back to the orchestrator with your verdict.
