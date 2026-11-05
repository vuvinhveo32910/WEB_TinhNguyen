# Dispatch Task for Reviewer M2.2

## Objective
Independently review Milestone 2 (Admin Product Management) implementation.
Perform adversarial review: check edge cases, security of file uploads, anti-forgery validation, deletion safety, and UI consistency with `_AdminLayout.cshtml`.
Run:
`XDG_RUNTIME_DIR=/tmp dotnet build /home/hutech/Vinh_Offline/WEB_TinhNguyen/WEB_TinhNguyen.slnx`
and:
`dotnet /home/hutech/Vinh_Offline/WEB_TinhNguyen/tests/E2ETests/bin/Debug/net10.0/E2ETests.dll --tier 4`
State your verdict explicitly: `APPROVE` or `REQUEST_CHANGES`.

Write report to `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_m2_2/handoff.md` and send message to orchestrator.


## 2026-11-03T13:43:31Z
You are teamwork_reviewer_m2_2.
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_m2_2
Your parent is the Project Orchestrator (Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe).

MANDATORY INSTRUCTIONS:
1. First read the original user request at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
2. Read the master project document at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md
3. Read your specific dispatch instructions at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_m2_2/DISPATCH.md
4. Perform an adversarial review of Milestone 2 files and verify compilation and test execution with `XDG_RUNTIME_DIR=/tmp`.
5. Write your handoff report to /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_reviewer_m2_2/handoff.md and state your verdict (APPROVE or REQUEST_CHANGES).
6. Send a message to the orchestrator (Recipient: 108b9d81-1dd6-495f-84eb-99dbb197aebe) when done.
