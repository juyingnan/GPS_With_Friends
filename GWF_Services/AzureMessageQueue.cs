using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;

namespace GWF_Services
{
    public class AzureMessageQueue: IMessageManager<string, GWFMessage>
    {
        private CloudQueue queue;

        public AzureMessageQueue(string name)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            this.queue = queueClient.GetQueueReference(name);
            queue.CreateIfNotExists();
        }

        public GWFMessage getMessage()
        {
            CloudQueueMessage peekedMessage = queue.GetMessage();
            GWFMessage gwf_msg = GWFMessage.deSerializeiFromString(peekedMessage.AsString);
            return gwf_msg;
        }

        public void deleteMessage(GWFMessage gwf_msg)
        {
            CloudQueueMessage message = new CloudQueueMessage(gwf_msg.serialize());
            queue.DeleteMessage(message);
        }

        public void addMessage(GWFMessage gwf_msg)
        {
            CloudQueueMessage message = new CloudQueueMessage(gwf_msg.serialize());
            queue.AddMessage(message);
        }
    }
}