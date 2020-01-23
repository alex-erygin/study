const { Client, logger } = require('camunda-external-task-client-js');

// configuration for the Client:
// - 'baseUrl': url to the Proess Engine
// - 'logger': utility to automatically log important events
// - 'asyncResponseTimeout': long poling timeout (then a new request will be issued)

const config = { baseUrl: 'http://localhost:8080/engine-rest', use: logger, asyncResponseTimeout: 10000 }

// create a Client instance with custom configuration
const client = new Client(config);

// susbscribe to the topic: 'charge-card'
client.subscribe('charge-card', async function({ task, taskService }) {
  // Put your business logic here
  // Get a process variable
  const amount = task.variables.get('amount');
  const item = task.variables.get('item');

  console.log(`Charging credit card with an amount of ${amount}€ for the item '${item}'...`);

  // Complete the task
  await taskService.complete(task);

  //await taskService.handleFailure(task, {Messagr:"Хана"});
});