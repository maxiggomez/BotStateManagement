using Microsoft.Bot.Schema;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotFramework
{
    public class Conversations : ConcurrentDictionary<string, ConversationReference>
    {
        public ConversationReference Get(string userName)
        {
            if (TryGetValue(userName, out ConversationReference value))
            {
                return value;
            }

            return null;
        }

        public void Save(ConversationReference conversationReference)
        {
            AddOrUpdate(
                //conversationReference.User.Name,
                conversationReference.ActivityId,
                conversationReference,
                (key, oldValue) => conversationReference);
        }
    }
}
