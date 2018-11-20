using System;
using System.Collections.Generic;
using System.Text;

namespace Main
{
    public class AppSettings
    {
        #region Service Bus Settings

        public const string ServiceBusConnectionString = "Endpoint=sb://sb-playground-ct-india.servicebus.windows.net/;SharedAccessKeyName=SWSharedAccessKey;SharedAccessKey=6gY8lEiwluap62tX2hF22CqT6ceC8U24P43pMOoMJm8=";
        public const string QueueName = "srinivas-playground";
        public const string TopicName = "srinivas-topic-playground";
        public const string SubscriptionName = "srinivas-topic-subscription-playground";

        #endregion

        #region Redis Cache Settings

        public const string CacheConnectionString = "127.0.0.1:6379";
        
        #endregion
    }
}
