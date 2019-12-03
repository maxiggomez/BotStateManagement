using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotFramework
{

    public enum QuestionOrder
    {
        WaitForUserInput,
        WaitForInputNumber,
        InProcess,
        Completed
    }

    public class TrackingState
    {
        public QuestionOrder Order = QuestionOrder.WaitForUserInput;
    }

    public class UserInfo
    {
        public string OrderNumber { get; set; }
        public String MessageId { get; set; }
    }


    public class StateAccessor
    {
        public ConversationState converstate;
        public IStatePropertyAccessor<TrackingState> currentTracking;

        public IStatePropertyAccessor<UserInfo> currentUser;

        public UserState userState;

        public StateAccessor()
        {
            converstate = new ConversationState(new MemoryStorage());
            currentTracking = converstate.CreateProperty<TrackingState>("TrackingState");

            userState = new UserState(new MemoryStorage());
            currentUser = userState.CreateProperty<UserInfo>("CurrentUser");
        }

    }
}
