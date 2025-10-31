## Purchasing MCP Tests

High-level tests for the Purchasing MCP server. This project validates core behaviors of tools/services exposed by `src/purchasing-mcp` without going deep into infrastructure concerns.

### What it covers (at a glance)

- Service and tool logic for supplier lookups and offer requests
- Basic request/response shapes and error paths
- Minimal integration seams (e.g., stubs/mocks for external calls)

### How to run

- Execute from this folder or the repository root using your standard .NET test workflow.
- Tests are designed to run locally without external dependencies.

Optional:

```powershell
dotnet test
```
