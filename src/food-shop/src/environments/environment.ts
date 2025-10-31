declare global {
  interface Window {
    env: any;
  }
}

export const environment = {
  production: true,
  title: 'Food Shop',
  authEnabled: true,
  mockCheckout: true,
  catalogApi: 'https://food-catalog-api-dev.azurewebsites.net/',
  ordersApi: 'http://localhost:5002/',
  azure: {
    apimSubscriptionKey: "",
    applicationInsights: ''
  },
  features: {
    logging: false,
    remoteCart: false,
    persistCart: false,
  }
};
