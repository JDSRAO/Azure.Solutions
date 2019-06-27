using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using Utils.Extensions;

namespace ServiceBus.Core
{
    static class MessageHelper
    {
        static int maxBodyLength = 196608;

        internal static IEnumerable<Message> GenerateBrokeredMessages(string message, bool useSessions)
        {
            var messages = new List<Message>();
            using (var messageStream = message.ToStream())
            {
                if (useSessions)
                {
                    string sessionId = Guid.NewGuid().ToString();
                    int totalMessages = (int)(messageStream.Length / maxBodyLength);
                    if (messageStream.Length % maxBodyLength > 0)
                    {
                        totalMessages++;
                    }

                    for (int streamOffest = 0, counter = 1; streamOffest < messageStream.Length; streamOffest += maxBodyLength, counter++)
                    {
                        var arraySize = (messageStream.Length - streamOffest) > maxBodyLength ? maxBodyLength : (messageStream.Length - streamOffest);
                        var subMessageBytes = new byte[arraySize];
                        messageStream.Read(subMessageBytes, 0, (int)arraySize);

                        var subMessage = new Message(subMessageBytes)
                        {
                            SessionId = sessionId,
                            MessageId = $"{sessionId}_{counter}",
                            ContentType = "application/json"
                        };
                        subMessage.UserProperties.Add(Constants.IsSplit, (totalMessages > 1));
                        subMessage.UserProperties.Add(Constants.SubMessageSequenceNumber, counter);
                        subMessage.UserProperties.Add(Constants.SubMessageTotalNumber, totalMessages);
                        messages.Add(subMessage);
                    }
                }
                else
                {
                    if (messageStream.Length > maxBodyLength)
                        throw new InvalidOperationException("Message is too big, either reduce the message size or use session for sending messages");

                    var messageBytes = new byte[messageStream.Length];
                    messageStream.Read(messageBytes, 0, messageBytes.Length);
                    var tempMessage = new Message(messageBytes)
                    {
                        MessageId = Convert.ToString(Guid.NewGuid()),
                        ContentType = "application/json"
                    };
                    tempMessage.UserProperties.Add(Constants.IsSplit, false);
                    messages.Add(tempMessage);
                }
            }
                
            return messages;
        }
    }
}
