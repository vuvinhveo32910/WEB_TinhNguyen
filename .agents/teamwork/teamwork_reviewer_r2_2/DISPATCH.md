# Task Dispatch: Reviewer R2_2 - Verification & Acceptance Criteria Review

## Role
Teamwork Reviewer (`teamwork_reviewer`)

## Working Directory
`/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_r2_2`

## Mandatory Reference Documents
- Authoritative User Request: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- Master Project Plan: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator_r2/PROJECT.md`
- Worker M2.1 Handoff: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_m2_1/handoff.md`
- Worker M2.2 Handoff: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_m2_2/handoff.md`
- Worker M2.3 Handoff: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_m2_3/handoff.md`

## Review Objective
Independently audit all requirements and acceptance criteria in `ORIGINAL_REQUEST.md`:
1. R1: Điều hướng & Giao diện cửa hàng gây quỹ (Storefront, Cart, Checkout, VietQR, responsive).
2. R2: Phân hệ quản trị Admin (Quản lý sản phẩm CRUD + Quản lý đơn hàng & quỹ tình nguyện).
3. R3: Tích hợp dữ liệu và Cấu trúc thư mục Area (`Areas/CuaHang`).
4. Follow-up: "tiep tuc hoang thien noi dung tren" - verify Admin order management, toast alerts, modals, live search/filter, 1-click copy, visual step tracker, stock inventory restock, and robust error handling.
5. Execute verification commands:
   - `export XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp HOME=/tmp; dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj --configfile tests/NuGet.Config --verbosity minimal`
   - `bash tests/run_e2e_tests.sh --all`

## Required Verdict
In your `handoff.md`, state clearly either:
`Verdict: APPROVE` or `Verdict: REQUEST_CHANGES (reason)`
and send a completion message with your verdict.

## 2026-11-04T01:15:44Z
You are teamwork_reviewer_r2_2.
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_r2_2
Authoritative user request record: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
Your task dispatch instructions are in: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_r2_2/DISPATCH.md

You MUST read /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md before starting work.

Your objective:
Conduct an independent acceptance criteria audit covering R1, R2, R3, and the follow-up request.
Run the build and all 310 tests:
`export XDG_RUNTIME_DIR=/tmp DOTNET_CLI_HOME=/tmp HOME=/tmp; dotnet build WEB_TinhNguyen/WEB_TinhNguyen.csproj --configfile tests/NuGet.Config --verbosity minimal`
`bash tests/run_e2e_tests.sh --all`
Inspect features and acceptance criteria.
Write your report and verdict (APPROVE / REQUEST_CHANGES) in:
`/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_r2_2/handoff.md`.
Send a completion message back to the orchestrator with your verdict.
