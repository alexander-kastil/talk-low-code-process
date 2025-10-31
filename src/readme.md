## src overview

This folder hosts the working examples and assets used across the low‑code/agents demos. It’s intentionally organized by capability so you can explore or reuse parts independently.

### What’s inside

- `agents/` – Copilot Studio agent projects exported to source (MCS). Each subfolder is a standalone agent with its own `agent.mcs.yml`, `settings.mcs.yml`, and optional actions/knowledge.
- `assets/` – Shared collateral (images, example MCP configs, OpenAPI specs, topic files) referenced by agents and services.
- `food-catalog-api/` – .NET minimal Web API that serves menu data; includes seeded data and optional Azure integrations. See its local README for details.
- `food-shop/` – Angular sample UI that consumes the food catalog (and related demos).
- `hr-mcp-server/` – C# MCP server exposing HR-related capabilities, built to work with the MCP Inspector and agents.
- `purchasing-mcp/` – C# MCP server for supplier lookups and price offers; current version.
- `purchasing-mcp-v1/` – Earlier iteration of the purchasing MCP server kept for comparison/history.
- `purchasing-mcp.tests/` – Unit tests for the purchasing MCP surface and helpers.
- `deploy-apis.azcli` – Convenience script(s) used during demos to deploy or manage API resources.

### How the pieces fit

Agents (in `agents/`) call into domain services via the Model Context Protocol (MCP). The MCP servers (HR and Purchasing) wrap business capabilities behind consistent tool schemas. For the food scenario, `food-catalog-api/` provides APIs and `food-shop/` offers a simple UI. Shared `assets/` supply prompts, images, and specs across scenarios.

### Getting started

- Browse into a subfolder and follow its README for run/debug steps.
- Prefer local READMEs for specifics (endpoints, commands, or environment setup) to keep this overview lightweight.
- If you deploy any of the APIs, the `deploy-apis.azcli` script can help automate common steps.

### Notes

- Backends target modern .NET (e.g., .NET 9) and use minimal hosting patterns.
- Frontend uses Angular; node and package versions follow the project’s `package.json`.
- MCP servers are compatible with the MCP Inspector for local or remote testing.
