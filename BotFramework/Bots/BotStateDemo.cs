using Microsoft.Bot.Builder;
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
        public BotStateDemo(StateAccessor stateAccessor)
        {
            _StateAccssor = stateAccessor;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = new CancellationToken())
        {
            var currentState = await _StateAccssor.currentTracking.GetAsync(turnContext, () => new TrackingState());

            var currentUser = await _StateAccssor.currentUser.GetAsync(turnContext, () => new UserInfo());

            if (turnContext.Activity == null || String.IsNullOrEmpty(turnContext.Activity.Text))
                return;


            switch (currentState.Order)
            {
                case QuestionOrder.WaitForUserInput:
                    if (turnContext.Activity.Text.ToUpper() == "ORDEN")
                    {
                        await turnContext.SendActivityAsync("Ingrese número de orden");
                        await ChangeState(turnContext, currentState, QuestionOrder.WaitForInputNumber);
                    }else
                        await turnContext.SendActivityAsync("No entiendo lo que ingresaste");

                    break;
                case QuestionOrder.WaitForInputNumber:
                    await turnContext.SendActivityAsync("Buscando datos de Orden de compra:" + turnContext.Activity.Text);
                    await ChangeState(turnContext, currentState, QuestionOrder.InProcess);

                    // Asigno el número de orden a buscar al usuario actual
                    await ChangeUserData(turnContext,currentUser, turnContext.Activity.Text);

                    // Voy a buscar datos de la orden a la API
                    string mensaje = await LlamarAPI(turnContext.Activity.Text);

                    await turnContext.SendActivityAsync(mensaje);

                    await ChangeState(turnContext, currentState, QuestionOrder.WaitForUserInput);
                    break;
                case QuestionOrder.InProcess:
                    // El número de orden que estoy procesando la obtengo del usuario
                    await turnContext.SendActivityAsync("Esperá unos minutos! Nos fuimos a buscar la info de la Orden número:" + currentUser.OrderNumber);
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

        private async Task<string> LlamarAPI(string orderNumber)
        {
            // Llamo a api
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string urlBase = "https://mggfunctionapptest.azurewebsites.net/api/FunctionTest?name=FunctionTest";
                    //string urlBase = "http://localhost:7071/api/FunctionTest?name=FunctionTest";

                    //Assuming that the api takes the user message as a query paramater
                    string RequestURI = urlBase + "&orderNumber=" + orderNumber;
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
