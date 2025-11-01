# Microsoft No-Code & Low-Code Agents: Intelligente Prozesse ohne Programmieraufwand

This repository contains demonstration materials and implementation examples for building intelligent process automation using Microsoft's no-code and low-code agent platforms, including Microsoft 365 Copilot Agents and Copilot Studio.

## 📋 Overview

This repository showcases how to create AI-powered agents that can automate business processes without extensive programming effort. It includes examples ranging from simple no-code agents to more sophisticated low-code implementations with custom connectors and MCP (Model Context Protocol) servers.

## 🗂️ Repository Structure

### 1. [Introduction](./01-introduction/)

Overview of Microsoft Copilots & Agents with links to:

- Microsoft Productivity Agents
- Microsoft Scenario Library
- HR Onboarding Agent examples and KPIs

### 2. [No-Code Examples](./02-no-code/)

**Employee Success Buddy** - A no-code agent built using Microsoft 365 Copilot Studio templates

- Career coaching and employee onboarding assistant
- Demonstrates knowledge integration from SharePoint
- Includes vacation request and hardware reporting forms
- No custom code required

### 3. [Low-Code Agent Examples](./03-low-code-agents/)

#### [Health Advisor](./03-low-code-agents/01-health-advisor/)

AI-powered health symptom analyzer that demonstrates:

- Knowledge upload (protein types document)
- Topic-based conversations
- AI Builder prompt integration for symptom summarization
- Microsoft Graph integration for user information
- Teams channel integration via Power Automate flows

#### [Returns Buddy](./03-low-code-agents/02-returns-buddy/)

Automated return request handler featuring:

- Policy document knowledge base
- Email trigger automation
- Automated email responses
- Integration with shared mailboxes

#### [Travel Planner](./03-low-code-agents/03.-travel-planner/)

Multi-agent business travel assistant showcasing:

- Parent-child agent architecture
- Focus agents with specialized knowledge
- Standalone connected agents
- Travel expense claim processing
- Activity recommendations and flight planning

#### [Food Order Buddy](./03-low-code-agents/04-food-order-buddy/)

Advanced agentic food ordering system demonstrating:

- Custom REST API connector (Food Catalog API)
- MCP (Model Context Protocol) integration (Purchasing MCP)
- Inventory management
- Supplier management
- Automated ordering based on demand
- Real-time stock checking

## 🛠️ Source Code Components

### [Agents](./src/agents/)

Cloned Copilot Studio agents created using the [Copilot Studio Visual Studio Code Extension](https://marketplace.visualstudio.com/items?itemName=ms-CopilotStudio.vscode-copilotstudio)

### [Food Catalog API](./src/food-catalog-api/)

.NET 9 minimal Web API providing CRUD operations for food items

- Entity Framework Core with SQL Server
- Azure integrations (Application Insights, Event Grid, Key Vault)
- Microsoft Entra ID authentication
- Swagger/OpenAPI documentation
- Feature flags for optional integrations

### [Purchasing MCP Server](./src/purchasing-mcp/)

.NET MCP server for supplier management and purchasing operations

- Supplier information API
- Price offer requests
- Order placement functionality
- MCP Inspector support for testing
- Azure deployment ready

### [Purchasing MCP Server v1](./src/purchasing-mcp-v1//)

- Simpler version of the Purchasing MCP Server that does not require a database

### [HR MCP Server](./src/hr-mcp-server/)

.NET MCP server for HR-related operations

- Employee data management
- HR workflow support
- MCP Inspector compatible
- Cloud deployment support

### [Food Shop](./src/food-shop/)

Frontend application for food ordering (Node.js/JavaScript)

## 🚀 Getting Started

### Prerequisites

- Microsoft 365 subscription with Copilot Studio access
- Azure subscription (for API deployments)
- .NET 9 SDK (for MCP servers and APIs)
- Node.js (for frontend applications)
- MCP Inspector: `npx @modelcontextprotocol/inspector`

### Running the Examples

#### No-Code Agents

Follow the instructions in [02-no-code](./02-no-code/) to create agents using Copilot Studio templates.

#### Low-Code Agents

Each low-code example in [03-low-code-agents](./03-low-code-agents/) contains detailed setup instructions.

#### MCP Servers

Navigate to the respective server directory and run:

```powershell
dotnet run
```

To test with MCP Inspector:

```powershell
npx @modelcontextprotocol/inspector --config inspector.config.json --server <server-name>
```

#### Food Catalog API

```bash
cd src/food-catalog-api
dotnet run
```

Access Swagger UI at the configured port.

## 🔗 Key Technologies

- **Microsoft 365 Copilot Studio** - Agent creation platform
- **Power Automate** - Workflow automation
- **Microsoft Graph** - User and organizational data access
- **Azure Application Insights** - Telemetry and monitoring
- **Azure Event Grid** - Event-driven architecture
- **MCP (Model Context Protocol)** - Standardized AI tool integration
- **.NET 9** - Backend services
- **Entity Framework Core** - Data access
- **OpenAPI/Swagger** - API documentation

## 📚 Resources

- [Microsoft Productivity Agents](https://www.microsoft.com/en-us/microsoft-365-copilot/agents)
- [Microsoft Scenario Library](https://adoption.microsoft.com/en-us/scenario-library/)
- [Copilot Studio VS Code Extension](https://marketplace.visualstudio.com/items?itemName=ms-CopilotStudio.vscode-copilotstudio)
- [MCP Inspector](https://github.com/modelcontextprotocol/inspector)

## 📄 License

Please refer to the repository license file for usage terms.

## 🤝 Contributing

This is a demonstration repository for showcasing Microsoft no-code and low-code agent capabilities. For questions or feedback, please use the repository's issue tracker.
