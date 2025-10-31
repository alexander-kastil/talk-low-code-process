export const environment = {
  production: false,
  title: 'Food Shop',
  authEnabled: true,
  mockCheckout: true,
  catalogApi: 'http://localhost:5000/',
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
