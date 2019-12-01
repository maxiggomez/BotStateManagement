using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotFramework
{
    public class MiddlewareLoggerHandler : IMiddleware
    {
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            await turnContext.SendActivityAsync("Paso antes por el middle", cancellationToken: cancellationToken);

            await next(cancellationToken);

            await turnContext.SendActivityAsync("Paso despúes por el middle", cancellationToken: cancellationToken);
        }
    }
}
