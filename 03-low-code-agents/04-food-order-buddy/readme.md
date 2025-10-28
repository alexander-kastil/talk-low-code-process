# Food Order Buddy

An agentic food ordering assistant that automates the ordering process.

## Main Implementation Components

- Custom Connector: Food Inventory REST
- Custom Connector: Purchasing MCP
- Topic: Check Symptoms
- Topic: Post to Teams
- Topic: Resolve E-Mail Address

### Custom Connector: Food Inventory REST

This connector interacts with the Food Catalog API to retrieve information about food items like: price, description, stock levels, min stock levels

- Show [Azure Deployment](https://food-catalog-api-dev.azurewebsites.net/index.html) explain Open Api definition

- Show sample [OpenApi](/src/assets/open-api/food-catalog-swagger-v2.yaml)

- Show connector definition in [Power Automate](https://make.powerautomate.com/environments/Default-d92b247e-90e0-4469-a129-6a32866c0d0a/connections/custom/)

- Demonstrate Food Inventory Agent:

  - Get all food
  - Add food to the inventory

    ```json
    {
    "name": "Margherita Pizza",
    "description": "Classic Italian pizza with fresh mozzarella, tomatoes, and basil.",
    "price": 8.99,
    "inStock": 50,
    "minStock": 10,
    "pictureUrl": "https://example.com/images/margherita-pizza.jpg"
    }
    ```

````

### Custom Connector: Purchasing MCP

This connector interacts with the Purchasing MCP to find suppliers, ask for inquiries and place orders based on food demands.

- Show [Azure Deployment](https://food-catalog-api-dev.azurewebsites.net/index.html) explain Open Api definition

- Show MCP Inspector:

    ```bash
    npx @modelcontextprotocol/inspector --config inspector.config.json --server purchasing-mcp-azure
    ```
- Show [OpenApi definition](/src/assets/mcp/purchasing-service.yaml)

- Demonstrate Purchasing Agent:

    - Show MCP Tools

### Food Order Buddy Sample Conversation

User: List all food in stock: name and quantity in stock, price in € as table

Food Order Buddy: Here is the list of food items in stock along with their availability:
...

User: List all suppliers with name and products they sell

Food Order Buddy: Here is the list of suppliers along with the products they sell:
...

User: Who sells Wiener Schnitzel?

Food Order Buddy: The suppliers that sell Wiener Schnitzel are:
...

User: Do we have Green Curry in stock?

Food Order Buddy: Yes, we currently have Green Curry in stock with a quantity of 17.

User: I have a new demand

> Note: Adaptive Card pops up to fill in details about the demand. In chose any food of your choice in a high quantity than currently in stock. I. e. Pad Ka Prao with quantity 66.

Food Order Buddy: Your demand was successfully noted

Food Order Buddy: Based on our current stock we have to order: [{"Product":"Pad Ka Prao","RequestedQuantity":48}]

Food Order Buddy: The offer for Pad Ka Prao has been successfully requested from the supplier "Same, same but different Foods Co., Ltd." The details are as follows:

Product: Pad Ka Prao
Requested Quantity: 48
Offered Price: $5.00 per unit
Transportation Cost: $30.00
Delivery Duration: 1 day
Would you like to proceed with placing the order?

User: What Austrian foods do we have in stock?
````
