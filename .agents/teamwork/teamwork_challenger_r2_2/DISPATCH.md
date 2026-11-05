# Task Dispatch: Challenger R2_2 - Storefront, AJAX & Security Stress Verification

## Role
Teamwork Challenger (`teamwork_challenger`)

## Working Directory
`/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_challenger_r2_2`

## Mandatory Reference Documents
- Authoritative User Request: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md`
- Master Project Plan: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator_r2/PROJECT.md`

## Verification Objective
Empirically challenge the storefront, cart, and UI/security aspects:
1. Verify AJAX endpoints in `GioHangController` (`ThemVaoGio`, `CapNhatSoLuong`, `XoaKhoiGio`) with unusual inputs (zero quantity, negative values, non-existent product IDs, SQL characters).
2. Verify VietQR payment generation and dynamic memo formatting.
3. Verify that Cart Badge, Floating Cart Button, and Tailwind Modals function cleanly across layouts.
4. Execute full test suite `bash tests/run_e2e_tests.sh --all` and report empirical findings.

## Required Verdict
`Verdict: APPROVE` or `Verdict: REQUEST_CHANGES (reason)`


## 2026-11-04T01:15:58Z
You are teamwork_challenger_r2_2.
Your working directory is: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_challenger_r2_2
Authoritative user request record: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md
Your task dispatch instructions are in: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_challenger_r2_2/DISPATCH.md

You MUST read /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md before starting work.

Your objective:
Empirically stress test storefront endpoints, AJAX cart requests, edge cases, VietQR dynamic parameters, modals, and toasts.
Run tests: `bash tests/run_e2e_tests.sh --all`
Write your findings and verdict (APPROVE / REQUEST_CHANGES) in:
`/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_challenger_r2_2/handoff.md`.
Send a completion message back to the orchestrator with your verdict.
