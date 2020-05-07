<Query Kind="Program">
  <NuGetReference>Microsoft.Azure.Storage.Common</NuGetReference>
  <NuGetReference>Microsoft.Azure.Storage.Queue</NuGetReference>
  <Namespace>Microsoft.Azure</Namespace>
  <Namespace>Microsoft.Azure.Storage</Namespace>
  <Namespace>Microsoft.Azure.Storage.Queue</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
	var connectionString = Util.GetPassword("sauron-storage-connection-string");
	connectionString.Dump();
	
	var storageAccount = CloudStorageAccount.Parse(connectionString);
	var queueClient = storageAccount.CreateCloudQueueClient();
	var queue = queueClient.GetQueueReference("the-queue");
	bool result = await queue.CreateIfNotExistsAsync().ConfigureAwait(false);
	$"Очередь создана {result}".Dump();
	
	var msg = new CloudQueueMessage("Hello !!!");
	var options = new QueueRequestOptions();
	var context = new OperationContext();
	context.ClientRequestID = Guid.NewGuid().ToString();
	await queue.AddMessageAsync(msg, TimeSpan.FromMinutes(5), null, options, context);
	"Сообщение добавлено".Dump();
	
	var receivedMsg = await queue.GetMessageAsync();
	"Сообщение прочитано".Dump();
	receivedMsg.AsString.Dump();
	
	await queue.DeleteMessageAsync(receivedMsg);
	"Сообщение удалено".Dump();
	
	await queue.DeleteAsync();
	"Очередь удалена".Dump();
}

// Define other methods, classes and namespaces here