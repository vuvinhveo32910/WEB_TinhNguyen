# Context & Environment

## Working Directory
- Root: `/home/hutech/Vinh_Offline/WEB_TinhNguyen`
- Orchestrator Working Directory: `/home/hutech/Vinh_Offline/WEB_TinhNguyen/.agents/teamwork/orchestrator_r2`
- User Request: `.agents/teamwork/ORIGINAL_REQUEST.md`

## Key Information
- Technology: ASP.NET Core MVC .NET 10.0, Entity Framework Core 10.0.12.
- Area: `Areas/CuaHang`
- Build command: `XDG_RUNTIME_DIR=/tmp dotnet build WEB_TinhNguyen.slnx`
- Test command: `XDG_RUNTIME_DIR=/tmp dotnet tests/E2ETests/bin/Debug/net10.0/E2ETests.dll`
- Previous Round 1 delivered core storefront, cart, admin product CRUD, VietQR payment integration, seed data, and in-memory dual fallback.
- Round 2 follow-up explicitly requests completing all remaining / uncompleted / improvable parts:
  - Admin order / donation tracking and status updates.
  - Enhanced UI/UX polish, notifications, toast messages, modal interactions, search/filtering.
  - Robustness and error handling.
  - Seed data, responsive layouts across devices.
  - Full test suite execution and verification.
