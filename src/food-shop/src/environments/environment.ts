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
    apimSubscriptionKey: "39fc8b24086a4346a6317d047869f983",
    applicationInsights: '89094b1f-dde1-4c07-8d40-f7d01ef18d55'
  },
  features: {
    logging: false,
    remoteCart: false,
    persistCart: false,
  }
};
