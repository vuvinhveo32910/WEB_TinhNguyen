# BRIEFING — 2026-11-04T03:36:30Z

## Mission
Tái cấu trúc Website Tình Nguyện ASP.NET Core MVC hướng tới môi trường production theo 4 phân hệ chính:
1. R1: Tái cấu trúc CSDL và dọn dẹp mã nguồn (EF Core Architect: xóa thuộc tính/cột màu, tạo Migration, Việt hóa toàn bộ bảng/cột và câu truy vấn LINQ/SQL).
2. R2: Việt hóa cấu trúc thư mục và cấu hình MVC (C# .NET 10 Dev: đổi Controllers->BoDieuKhien, Models->MoHinh, Views->GiaoDien, Data->DuLieu, cấu hình Razor ViewEngine & Routing).
3. R3: Đổi mới giao diện người dùng Tailwind CSS hiện đại, truyền cảm hứng, animations, nền trắng chữ chủ đề tình nguyện, bảo toàn 100% phân hệ bán hàng (Areas/CuaHang/) (UI/UX Specialist).
4. R4: Tính năng chuyển đổi Sáng/Tối (Light/Dark Mode) qua JS & CSS variables cho toàn trang trừ Areas/CuaHang/.

## 🔒 My Identity
- Archetype: Project Orchestrator
- Roles: orchestrator, user_liaison, human_reporter, successor
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator
- Original parent: Sentinel / Parent agent
- Original parent conversation ID: b3e3006a-fa2e-49bf-8124-bab9842d493f

## 🔒 My Workflow
- **Pattern**: Project
- **Scope document**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md
1. **Decompose**: Survey with 3 Explorers (Completed). Decomposed into M1, M2, M3, M4 + E2E Testing Track.
2. **Dispatch & Execute**:
   - **Direct (iteration loop)**: Explorer (3) -> Worker (1) -> Reviewer (2) -> Challenger (2) -> Auditor (1) -> Gate
3. **On failure**: Retry -> Replace -> Skip -> Redistribute -> Redesign
4. **Succession**: Self-succeed when threshold reached and replacement allowed
- **Work items**:
  1. Survey & Architecture Mapping [done]
  2. E2E Test Suite Track [done: 85 Tier 1 + 25 Tier 2 + 16 Tier 3 + 12 Tier 4 + 142 Tier 5 = 280 tests]
  3. Milestone 1: Area & Data Foundation [done: PASS]
  4. Milestone 2: Admin Product Management [done: PASS]
  5. Milestone 3: Storefront, Cart & Navigation [done: PASS]
  6. Milestone 4: Final E2E Pass & Adversarial Hardening [done: PASS (280/280 tests, Forensic Auditor CLEAN)]
- **Current phase**: Complete (All Milestones M1 - M5 Passed)
- **Current focus**: Final Human Reporting & Handoff to Parent Agent
- **Refactoring Work Items (Round 3)**:
  1. Survey & Codebase Investigation (3 Explorers) [completed]
  2. Milestone M1: Việt hóa Cấu trúc Thư mục & Razor ViewEngine (R2) [completed: 100% GATE PASS]
  3. Milestone M2: Tái cấu trúc CSDL & Dọn dẹp Mã nguồn (R1) [completed: 100% GATE PASS]
  4. Milestone M3: Giao diện Tailwind CSS & Bảo toàn CuaHang (R3) [completed: 100% GATE PASS]
  5. Milestone M4: Chuyển đổi Sáng/Tối Light/Dark Mode (R4) [completed: 100% GATE PASS]
  6. Milestone M5: E2E Testing, Adversarial Verification & Forensic Audit Gate [completed: 100% GATE PASS]

## 🔒 Key Constraints
- NEVER write, modify, or create source code files directly.
- NEVER run build/test commands yourself — require workers to do so.
- NEVER investigate or explore the problem at the code level — dispatch Explorers for technical investigation.
- You MAY use file-editing tools ONLY for metadata/state files (.md) in your .agents/teamwork/ folder.
- Never reuse a subagent after it has delivered its handoff — always spawn fresh
- Hard veto on forensic auditor failure.
- Must use ASP.NET Core Area.
- Must compile cleanly with dotnet build (use XDG_RUNTIME_DIR=/tmp in environment).
- Integrity mode: development.
- R1: Xóa toàn bộ thuộc tính lưu màu chữ, màu nền trong Models và DbContext. Tạo và chạy Code First Migration. Đổi tên toàn bộ bảng, cột sang tiếng Việt, cập nhật LINQ/SQL.
- R2: Đổi tên thư mục Controllers, Models, Views, Data sang tiếng Việt (BoDieuKhien, MoHinh, GiaoDien, DuLieu). Cấu hình Razor ViewEngine và routing chuẩn.
- R3: Làm mới toàn bộ UI với Tailwind CSS, animations mượt mà, chủ đề tình nguyện. Nền trắng mặc định, chữ màu hài hòa. Bảo toàn 100% cấu trúc MVC/DOM và giao diện hiện tại của Areas/CuaHang/.
- R4: Nút Sáng/Tối bằng JS + CSS variables trên layout chung (trừ Areas/CuaHang/).

## Current Parent
- Conversation ID: 13e142e6-9ec4-415f-8ba5-0e0f474aa0f8
- Updated: 2026-11-04T06:03:02Z

## Key Decisions Made
- Milestone M1 completed and gate verified with PASS (Reviewers APPROVE, Auditor CLEAN, 316/316 E2E tests pass).
- Milestone M2 completed and gate verified with PASS (Auditor CLEAN, 2 Reviewers APPROVE, 2 Challengers APPROVE, 325/325 tests pass).
- Milestone M3 completed and gate verified with PASS (Auditor CLEAN, 2 Reviewers APPROVE, 2 Challengers APPROVE, 325/325 E2E tests pass, 128/128 stress tests pass).
- Milestone M4 completed and gate verified with PASS (Auditor CLEAN, 2 Reviewers APPROVE, 2 Challengers APPROVE, 337/337 E2E tests pass, 128/128 M3 tests pass, 67/67 M1 tests pass, 109/109 adversarial checks pass).
- Milestone M5 completed and gate verified with PASS (Auditor CLEAN, 2 Reviewers APPROVE, 2 Challengers APPROVE, 733/733 tests pass, 0 Error). All acceptance criteria satisfied.

## Team Roster
| Agent | Type | Work Item | Status | Conv ID |
|-------|------|-----------|--------|---------|

## Succession Status
- Succession required: no
- Spawn count: 72 / 128
- Pending subagents: none
- Predecessor: none
- Successor: none

## Active Timers
- Heartbeat cron: none (all tasks completed and cancelled)
- Safety timer: none




## Artifact Index
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md — Original User Request
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md — Global project specification & milestones
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/GATE_STATUS.md — Gate status tracking
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_worker_r3_m2/DISPATCH.md — Worker M2 Task Instructions
- /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_1/handoff.md — Database Architect Survey Blueprint
