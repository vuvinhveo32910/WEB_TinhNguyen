#!/usr/bin/env bash
set -euo pipefail

# -----------------------------------------------------------------------------
# WEB_TinhNguyen E2E Test Suite Standalone Runner
# -----------------------------------------------------------------------------
# Runs Tiers 1-4 E2E requirement-driven tests.
# Supports passing arguments directly to the runner engine:
#   ./run_e2e_tests.sh --all
#   ./run_e2e_tests.sh --tier 1
#   ./run_e2e_tests.sh --feature 3
#   ./run_e2e_tests.sh --filter Keyword
#   ./run_e2e_tests.sh --verbose
# -----------------------------------------------------------------------------

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SOLUTION_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

export XDG_RUNTIME_DIR=/tmp
export DOTNET_CLI_HOME=/tmp
export HOME=/tmp

CONFIG_FILE="$SCRIPT_DIR/NuGet.Config"
PROJECT_FILE="$SCRIPT_DIR/E2ETests/E2ETests.csproj"

if [ ! -f "$PROJECT_FILE" ]; then
    echo "Error: E2E project file not found at $PROJECT_FILE" >&2
    exit 1
fi

echo "========================================================================"
echo " Building WEB_TinhNguyen E2E Test Suite..."
echo "========================================================================"
dotnet build "$PROJECT_FILE" --configfile "$CONFIG_FILE" --verbosity minimal

echo ""
echo "========================================================================"
echo " Executing WEB_TinhNguyen E2E Test Suite..."
echo "========================================================================"
if [ $# -eq 0 ]; then
    dotnet run --no-build --project "$PROJECT_FILE" -- --all
else
    dotnet run --no-build --project "$PROJECT_FILE" -- "$@"
fi
