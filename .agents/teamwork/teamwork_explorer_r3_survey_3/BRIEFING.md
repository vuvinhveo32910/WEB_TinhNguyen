# BRIEFING — 2026-11-04T02:45:00Z

## Mission
Investigate codebase UI/UX structure, styles, layouts, CuaHang isolation, and design Tailwind CSS theme + Light/Dark mode for Requirements R3 & R4.

## 🔒 My Identity
- Archetype: Explorer
- Roles: UI/UX & Tailwind Specialist Explorer (Chuyên gia UI/UX - Tailwind CSS, Theme, Dark Mode & Isolation)
- Working directory: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_3
- Original parent: c18bf841-3fbf-4393-8780-5f56ef90fb21
- Milestone: Investigation & Blueprint for R3 & R4

## 🔒 Key Constraints
- Read-only investigation — do NOT implement / modify source code directly
- Must design modern, inspiring volunteer identity, smooth animations with Tailwind CSS
- Default white background (`bg-white`) with harmonious volunteer-themed typography and palette
- Areas/CuaHang/ 100% MVC/DOM and existing visual appearance preserved untouched & strictly isolated
- Light/Dark mode toggle on shared layout using JS & CSS variables (or Tailwind dark: / data-theme); Light: white bg + volunteer text; Dark: dark bg + light text; NEVER affect CuaHang
- Produce structured 5-component handoff report

## Current Parent
- Conversation ID: c18bf841-3fbf-4393-8780-5f56ef90fb21
- Updated: 2026-11-04T02:45:00Z

## Investigation State
- **Explored paths**: `Views/Shared/_Layout.cshtml`, `Views/Shared/_AdminLayout.cshtml`, `Views/Home/*`, `Areas/CuaHang/Views/*`, `package.json`, `tailwind.config.js`, `wwwroot/css/site.css`, `styles.css`, `Program.cs`.
- **Key findings**: 
  1. Found build output path mismatch in `package.json` (`-o wwwroot/css/site.css` vs `WEB_TinhNguyen/wwwroot/css/site.css`).
  2. Identified database color dependency in `_Layout.cshtml` and `Home/*` that will break with R1; defined CSS variables system to decouple completely.
  3. Formulated "Volunteer Identity" palette: default white background (`#ffffff`), deep volunteer forest text (`#143823`), leaf bright (`#22c55e`), warm amber (`#f59e0b`), coral rose (`#f43f5e`).
  4. Designed Light/Dark mode with clean header icon toggle, CSS variables, and anti-FOUC script.
  5. Established 100% CuaHang isolation architecture via dedicated `_CuaHangLayout.cshtml` and area route guard.
- **Unexplored areas**: None within the R3/R4 scope.

## Key Decisions Made
- Deliver comprehensive technical blueprint covering Volunteer Identity Palette, Animations, CSS Variables, Theme Toggle Component, CuaHang Dedicated Layout, and Build Script Path Fix in `handoff.md`.

## Artifact Index
- DISPATCH.md — Initial mission dispatch
- BRIEFING.md — Persistent context & state
- progress.md — Liveness heartbeat and milestone tracker
- handoff.md — Authoritative 5-component report and architecture blueprint
