# Dispatch Task for E2E Test Writer

## Objective
Design and implement the E2E test suite for the volunteer fundraising store system per requirements in ORIGINAL_REQUEST.md and PROJECT.md § Feature Inventory.
Publish TEST_INFRA.md and TEST_READY.md when test infrastructure and test cases are ready.

## Test Philosophy & Methodology
- Opaque-box, requirement-driven. Derive tests from user requirements, NOT implementation internals.
- Implement 4-tier methodology:
  - Tier 1: Feature Coverage (>=5 test cases per feature for all 17 features = >=85 tests or complete coverage of all key features)
  - Tier 2: Boundary & Corner Cases (empty cart, max stock, zero donation, invalid phone/email, long text, etc.)
  - Tier 3: Cross-Feature Combinations (add to cart -> update qty -> checkout -> verify order & stock; admin create -> storefront verify -> admin edit -> storefront verify -> admin delete)
  - Tier 4: Real-World Scenarios (complete donor purchase flow, complete admin management flow, responsive UI element checks)
- Provide a standalone test runner script (e.g. `tests/run_e2e_tests.sh` or C#/bash runner) executable with exit code 0 on all tests passing.
- Test runner MUST use `XDG_RUNTIME_DIR=/tmp` for any `dotnet` commands.
- Publish `TEST_INFRA.md` in `.agents/teamwork/teamwork_test_writer_e2e/TEST_INFRA.md` and publish `TEST_READY.md` in `.agents/teamwork/teamwork_test_writer_e2e/TEST_READY.md`.

## Input Files
- `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md`

## 2026-11-03T12:43:11Z
You are teamwork_test_writer_e2e.
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_test_writer_e2e
Your parent is the Project Orchestrator (Conversation ID: 108b9d81-1dd6-495f-84eb-99dbb197aebe).

MANDATORY INSTRUCTIONS:
1. First read the original user request at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
2. Read the master project document at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator/PROJECT.md
3. Read your specific dispatch instructions at: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_test_writer_e2e/DISPATCH.md
4. Update your progress.md regularly with "Last visited: [timestamp]" as your heartbeat.
5. Build the comprehensive E2E test suite covering Tiers 1-4 for all features in PROJECT.md § Feature Inventory. Create test files and executable runner script (e.g. tests/run_e2e_tests.sh). Ensure any dotnet commands use `XDG_RUNTIME_DIR=/tmp`.
6. Write TEST_INFRA.md and TEST_READY.md in your working directory and summarize coverage.
7. Write handoff.md following the Handoff Protocol.
8. When finished, send a message to the orchestrator (Recipient: 108b9d81-1dd6-495f-84eb-99dbb197aebe).

## 2026-11-03T13:18:45Z
**Context**: E2E Test Track Finalization
**Content**: The E2E test suite in tests/E2ETests and tests/run_e2e_tests.sh is built and verified. Please publish TEST_INFRA.md and TEST_READY.md in your working directory and submit your handoff.md report.
**Action**: Write TEST_INFRA.md, TEST_READY.md, handoff.md and send completion message.
