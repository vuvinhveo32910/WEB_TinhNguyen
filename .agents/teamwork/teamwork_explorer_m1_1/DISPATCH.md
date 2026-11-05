# Dispatch Task for M1 Explorer 1 (Program.cs, DbContext & Tailwind)

## Objective
Analyze and specify exact implementation details for:
1. `WEB_TinhNguyen/Program.cs`: Adding DistributedMemoryCache, Session service with options, HttpContextAccessor, `app.UseSession()`, and Area route `{area:exists}/{controller=CuaHang}/{action=Index}/{id?}`.
2. `WEB_TinhNguyen/Data/ApplicationDbContext.cs`: Adding DbSets for `SanPhamGayQuy`, `DonHangGayQuy`, `ChiTietDonHangGayQuy`.
3. `tailwind.config.js`: Adding `"./Areas/**/*.cshtml"` to `content` array.
4. Verify compilation command `XDG_RUNTIME_DIR=/tmp dotnet build`.

## Input Files
- `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`

## Output
Write detailed design to `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_1/handoff.md`.

## 2026-11-03T12:43:31Z
You are teamwork_explorer_m1_1.
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_1
Your parent is the Project Orchestrator (Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe).

MANDATORY INSTRUCTIONS:
1. First read the original user request at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
2. Read the master project document at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md
3. Read your specific dispatch instructions at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_1/DISPATCH.md
4. Update your progress.md regularly with "Last visited: [timestamp]" as your heartbeat.
5. Provide exact implementation designs for Program.cs, ApplicationDbContext.cs, and tailwind.config.js for Milestone 1.
6. Write your report to /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_m1_1/handoff.md following the Handoff Protocol.
7. When finished, send a message to the orchestrator (Recipient: 108b9d81-1dd6-495f-84eb-99dbb197aebe).
