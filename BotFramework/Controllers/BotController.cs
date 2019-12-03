// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;

namespace BotFramework.Controllers
{
    // This ASP Controller is created to handle a request. Dependency Injection will provide the Adapter and IBot
    // implementation at runtime. Multiple different IBot implementations running at different endpoints can be
    // achieved by specifying a more specific type for the bot constructor argument.
    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter Adapter;
        private readonly IBot Bot;
        private readonly Conversations _conversations;
        public BotController(IBotFrameworkHttpAdapter adapter, IBot bot, Conversations conversations)
        {
            Adapter = adapter;
            Bot = bot;
            _conversations = conversations ?? throw new ArgumentNullException(nameof(conversations));
        }

        [HttpPost, HttpGet]
        public async Task PostAsync()
        {
            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.
            await Adapter.ProcessAsync(Request, Response, Bot);
        }


        [HttpPost("/events/{id}")]
        public async Task<InvokeResponse> Events([FromRoute]string id)
        {
            string body = null;

            //userName = WebUtility.UrlDecode(userName);

            //using (var reader = new StreamReader(ControllerContext.HttpContext.Request.Body))
            //{
            //    // quick and dirty sanitization
            //    body = (await reader.ReadToEndAsync())
            //        .Replace("script", "", StringComparison.InvariantCultureIgnoreCase)
            //        .Replace("href", "", StringComparison.InvariantCultureIgnoreCase);
            //}

            // _logger.LogTrace("----- BotController - Receiving event: \"{EventName}\" - user: \"{UserName}\" ({Body})", eventName, userName, body);

            // Acá deberiamos buscar por el id que llega en el request que deberia ser el mismo que el de la conversacion
            var conversation = _conversations.Get("User");

            if (conversation == null)
            {
                return new InvokeResponse { Status = 404, Body = body };
            }

            await ((BotAdapter)Adapter).ContinueConversationAsync("eqwe", conversation, async (context, token) =>
            {
                context.Activity.Name = "hoal";
                context.Activity.Text = "Orden 1 - 5000 pesos";
                context.Activity.Type = "event";
                context.Activity.RelatesTo = null;
                context.Activity.Value = body;

                //_logger.LogTrace("----- BotController - Craft event activity: {@Activity}", context.Activity);

                await Bot.OnTurnAsync(context, token);
            },default);

            await Adapter.ProcessAsync(Request, Response, Bot);

            //var botAppId = "74e5a522-355b-4266-ad53-6e02fc1fe2d5"; // _configuration["BotWebApiApp:AppId"];

            //await _adapter.ContinueConversationAsync(botAppId, conversation, async (context, token) =>
            //{
            //    context.Activity.Name = "nombreEvento";
            //    context.Activity.RelatesTo = null;
            //    context.Activity.Value = body;

            //   // _logger.LogTrace("----- BotController - Craft event activity: {@Activity}", context.Activity);

            //    await Bot.OnTurnAsync(context, token);
            //});

            return new InvokeResponse { Status = 200, Body = body };
        }

    }
}
