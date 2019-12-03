using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BotFramework.Bots
{
    public class BotStateDemo : IBot
    {
        private StateAccessor _StateAccssor;
        private readonly Conversations _conversations;
        public BotStateDemo(StateAccessor stateAccessor, Conversations conversations)
        {
            _StateAccssor = stateAccessor;
            _conversations = conversations;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = new CancellationToken())
        {
            var currentState = await _StateAccssor.currentTracking.GetAsync(turnContext, () => new TrackingState());

            var currentUser = await _StateAccssor.currentUser.GetAsync(turnContext, () => new UserInfo());

            if (turnContext.Activity == null || String.IsNullOrEmpty(turnContext.Activity.Text))
                return;

            var activityType = turnContext.Activity.Type;
            var conversationReference = (ConversationReference)null;

            if(turnContext.Activity.Text.ToUpper() == "CANCELAR")
            {
                await turnContext.SendActivityAsync("Consulta cancelada");
                await ChangeState(turnContext, currentState, QuestionOrder.WaitForUserInput);
                return;
            }


            if (activityType == ActivityTypes.Message || activityType == ActivityTypes.ConversationUpdate)
            {
                conversationReference = turnContext.Activity.GetConversationReference();

                _conversations.Save(conversationReference);
            }
            else
            {
                if (activityType == ActivityTypes.Event) // es un evento de afuera
                {
                    // Solo lo proceso si estoy esperando una respuesta
                    if(currentState.Order == QuestionOrder.InProcess)
                    {
                        await turnContext.SendActivityAsync(turnContext.Activity.Text);
                        await ChangeState(turnContext, currentState, QuestionOrder.WaitForUserInput);
                    }

                    return;
                }

            }


            switch (currentState.Order)
            {
                case QuestionOrder.WaitForUserInput:
                    if (turnContext.Activity.Text.ToUpper() == "ORDEN")
                    {
                        await turnContext.SendActivityAsync("Ingrese número de orden");
                        await ChangeState(turnContext, currentState, QuestionOrder.WaitForInputNumber);
                    }
                    else
                        await turnContext.SendActivityAsync("No entiendo lo que ingresaste");

                    break;
                case QuestionOrder.WaitForInputNumber:
                    await turnContext.SendActivityAsync("Buscando datos de Orden de compra:" + turnContext.Activity.Text);
                    await ChangeState(turnContext, currentState, QuestionOrder.InProcess);

                    //var messageId = Guid.NewGuid();
                    currentUser.MessageId = conversationReference.ActivityId;

                    // Asigno el número de orden a buscar al usuario actual
                    await ChangeUserData(turnContext, currentUser, turnContext.Activity.Text);

                    // Voy a buscar datos de la orden a la API
                    // FALTA GENERAR EL ID
                    string mensaje = await LlamarAPI(turnContext.Activity.Text, currentUser.MessageId);

                    // Descomentar cuando se quiera probar directamente esperar la respeusta de la API
                    //await turnContext.SendActivityAsync(mensaje);
                    //await ChangeState(turnContext, currentState, QuestionOrder.WaitForUserInput);
                    break;
                case QuestionOrder.InProcess:
                    // El número de orden que estoy procesando la obtengo del usuario
                    await turnContext.SendActivityAsync(String.Format("Esperá unos minutos!\r\n Nos fuimos a buscar la info de la Orden número:{0}\r\nIngrese cancelar para detener la consulta", currentUser.OrderNumber));
                    break;
                default:
                    break;
            }

        }

        private async Task ChangeState(ITurnContext turnContext, TrackingState currentState, QuestionOrder newState)
        {
            currentState.Order = newState;
            await _StateAccssor.currentTracking.SetAsync(turnContext, currentState);
            await _StateAccssor.converstate.SaveChangesAsync(turnContext);


        }

        private async Task ChangeUserData(ITurnContext turnContext, UserInfo currentUser, string data)
        {

            currentUser.OrderNumber = turnContext.Activity.Text;
            await _StateAccssor.currentUser.SetAsync(turnContext, currentUser);
            await _StateAccssor.userState.SaveChangesAsync(turnContext);

        }

        private async Task<string> LlamarAPI(string orderNumber, string messageId)
        {
            // Llamo a api
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //string urlBase = "https://mggfunctionapptest.azurewebsites.net/api/FunctionTest?name=FunctionTest";
                    string urlBase = "http://localhost:7071/api/FunctionTest?name=FunctionTest";

                    //Assuming that the api takes the user message as a query paramater
                    string RequestURI = urlBase + "&orderId=" + orderNumber + "&messageId=" + messageId;
                    HttpResponseMessage responsemMsg = await client.GetAsync(RequestURI);
                    if (responsemMsg.IsSuccessStatusCode)
                    {
                        var apiResponse = await responsemMsg.Content.ReadAsStringAsync();

                        //Post the API response to bot again
                        //await context.PostAsync($"Response is {apiResponse}");
                        return apiResponse;
                    }
                    else
                        return "error general";
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }

        }

    }

}
