// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            string body = await new StreamReader(Request.Body).ReadToEndAsync();

            var valores = JsonConvert.DeserializeObject<OrdenMessage>(body);

            var conversation = _conversations.Get(valores.MesssageId);

            if (conversation == null)
            {
                return new InvokeResponse { Status = 404, Body = body };
            }

            await ((BotAdapter)Adapter).ContinueConversationAsync("botId", conversation, async (context, token) =>
            {
                context.Activity.Name = "orderRPAResult";
                context.Activity.Text = String.Format("Resultado recibido para MensajeId:{0}\r\nOrden:{1}\r\nMonto:{2}\r\nEstado:{3}",valores.MesssageId,valores.OrdenId,valores.Total,valores.Estado);
                context.Activity.Type = "event";
                context.Activity.RelatesTo = null;
                //context.Activity.Value = body;

                await Bot.OnTurnAsync(context, token);
            },default);

            await Adapter.ProcessAsync(Request, Response, Bot);

            return new InvokeResponse { Status = 200, Body = body };
        }

    }

    public class OrdenMessage
    {
        public string OrdenId { get; set; }
        public string MesssageId { get; set; }
        public string Total { get; set; }
        public string Estado { get; set; }
    }
}
