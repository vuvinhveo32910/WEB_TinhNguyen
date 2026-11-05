# DISPATCH for Explorer 3 (UI/UX & Tailwind CSS Specialist)

**Role**: UI/UX & Tailwind Specialist Explorer (Chuyên gia UI/UX - Tailwind CSS, Theme, Dark Mode & Isolation)
**Working Directory**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_3
**Original Request**: /home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/ORIGINAL_REQUEST.md

## Mission & Scope
Investigate codebase for Requirements R3 & R4:
1. Examine all existing Views (`Views/Shared/_Layout.cshtml`, `Views/Home/`, `Views/Admin/`, etc.), CSS styles (`wwwroot/css/`), scripts (`wwwroot/js/`).
2. Explore integrating Tailwind CSS (e.g. Tailwind via CDN with plugins/custom config or build pipeline) with modern, inspiring volunteer identity, smooth animations.
3. Design default white background (`bg-white`) with harmonious volunteer-themed typography and palette.
4. Investigate `Areas/CuaHang/` thoroughly:
   - Identify all views, layouts, CSS files used by `Areas/CuaHang/`.
   - Provide concrete isolation architecture so that 100% of the MVC/DOM structure and existing visual appearance of `Areas/CuaHang/` is preserved untouched, completely isolated from the new Tailwind styles.
5. Investigate Light/Dark Mode (R4):
   - Button design and placement on shared layout.
   - Mechanism using JavaScript and CSS variables (`--bg-primary`, `--text-primary`, `--accent`, etc.) or Tailwind `dark:` / `data-theme`.
   - Behavior: Light Mode (default white bg + volunteer text color); Dark Mode (dark bg + light text).
   - Strict isolation: ensure Dark Mode toggle NEVER affects `Areas/CuaHang/`.
6. Write findings and technical blueprint to `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/teamwork_explorer_r3_survey_3/handoff.md`.
