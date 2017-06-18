namespace LuisBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json.Linq;
    using LuisBot.FormFlow;
    using LuisBot.CommonTypes;
    using static LuisBot.CommonTypes.CommonTypes;

    [LuisModel("77fee18b-8bbe-4e7b-b054-d863143ab616", "31aa162117554361b119f1a76e403f85")]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"סליחה, לא ממש הבנתי למה התכוונת ב'{result.Query}'. אפשר לרשום עזרה כדי לקבל מידע.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }


        [LuisIntent("ApproveCourseEnrollment")]
        public async Task ApproveCourseEnrollmentIntent(IDialogContext context, LuisResult result)
        {

            //if (result.Entities.Count == 0)
            //{
            //    await context.PostAsync("חסרים פרטים באישור בקשת ההשתלמות");
            //}
            // else
            if (result.Entities.Where(i => i.Type == ("EnrollmentRequestID")).FirstOrDefault() == null)
            {
                await context.PostAsync("חסרים פרטים באישור בקשת ההשתלמות");
                //  EnrollmentApprovalQuery  enrollmentApprovalQuery = new EnrollmentApprovalQuery();
                context.Wait(this.MessageReceived);
            }
            else
            {
                string message = $"ביקשת לאשר בקשת השתלמות, אנא המתן...";
                // context.Call<object>(new CoursesDialog(), AfterCourseDialogIsDone);
                //gather the data.
                ActionItem infoToSend = new ActionItem()
                {
                    //  ActionItemMask = ActionItemMask.None,
                    ActionToTake = ActionToTake.Enquire,//"ApproveCourseEnrollment",
                    MethodName = "GetCourseDetailsByID",
                    ParametersList = new List<ActionParameters>
                    {
                        new ActionParameters { ParameterName = "EnrollmentRequestID", ParameterType = "Int", ParameterValue = result.Entities.Where(i => i.Type == ("EnrollmentRequestID")).FirstOrDefault().Entity }
                    },
                    SystemName = "CoursesEnrollmentSystem",
                    WsUrl = "https://somekindOfWebAddress/blabla.asmx" //TODO: Replace with url

                };
                //call web service with this info.

                //Get back an answer and handle it.
                // await context.PostAsync(" . 'האם אתה מעוניין לאשר את בקשת ההשתלמות בשם 'שם כלשהו שחזר מהשירות'? הקלד 'אשר' או 'דחה' ");

                //Set the EnrollmentRequestID 
                context.ConversationData.SetValue(ContextConstants.CN_EnrollmentRequestID, infoToSend.ParametersList[0].ParameterValue);
                // context.Wait(AwaitingConfirmation);
                PromptDialog.Text(context, ResumeAfterPrompt, "'האם אתה מעוניין לאשר את בקשת ההשתלמות בשם 'שם כלשהו שחזר מהשירות'? הקלד 'אשר' או 'דחה או 'ביטול פעולה' ' ");
                //PromptDialog.Choice()
                //What if i couldnt find the number? suggest others to approve or reject

            }

            // context.Wait(this.MessageReceived);
        }

        private async Task ResumeAfterPrompt(IDialogContext context, IAwaitable<string> result)
        {

            var userReply = await result;

            if (userReply != "Which" && userReply != "Reject" && userReply != "Undo")
            {
                PromptDialog.Text(context, ResumeAfterPrompt, "'האם אתה מעוניין לאשר את בקשת ההשתלמות בשם 'שם כלשהו שחזר מהשירות'? הקלד 'אשר' או 'דחה או 'ביטול פעולה' ' ");
            }

            switch (userReply)
            {
                case "Which": //אשר
                    {

                        //send approval to the web service
                        ActionItem infoToSend = new ActionItem()
                        {
                            //  ActionItemMask = ActionItemMask.None,
                            ActionToTake = ActionToTake.Approve,//"ApproveCourseEnrollment",
                            MethodName = "ApproveCourseEnrollment",
                            ParametersList = new List<ActionParameters>
                                       {
                                     new ActionParameters { ParameterName = "EnrollmentRequestID", ParameterType = "Int",
                                      ParameterValue = context.ConversationData.GetValue<string>(ContextConstants.CN_EnrollmentRequestID)
                                         }
                                  },
                            SystemName = "CoursesEnrollmentSystem",
                            WsUrl = "https://somekindOfWebAddress/blabla.asmx" //TODO: Replace with url

                        };

                        //send to the web service
                        await context.PostAsync($"פעולת האישור נשלחה ותטופל.");
                    }
                    break;

                case "Reject"://דחה
                    await context.PostAsync($"פעולת הדחייה נשלחה ותטופל.");
                    break;

                case "Undo"://ביטול פעולה
                    await context.PostAsync($"הבקשה בוטלה.");
                    break;
                default:
                    break;
            }


        }

        private async Task AfterCourseDialogIsDone(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("האם תרצה לבצע משהו נוסף?");
            // await context.PostAsync(message + "asdasd");
            context.Wait(this.MessageReceived);
        }



        [LuisIntent("Todovum")]
        public async Task TodovumTodovum(IDialogContext context, LuisResult result)
        {
            Attachment attachment = new Attachment();
            attachment.ContentType = "image/png";
            attachment.ContentUrl = "http://images.nana10.co.il/upload/mediastock/img/16/0/287/287033.jpg";
            var message = context.MakeMessage();
            message.Attachments.Add(attachment);
            message.Text = " (: טודו טודו בום, הכל בסדר, טודובום";
            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("OpenFaultTicket")]
        public async Task OpenFaultTicket(IDialogContext context, LuisResult result)
        {
            string message = $"";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("GreetingIntent")]
        public async Task GreetingIntent(IDialogContext context, LuisResult result)
        {

            await context.PostAsync(GetGreeting());

        }
        [LuisIntent("Weather")]
        public async Task Weather(IDialogContext context, LuisResult result)
        {
            if (result.Entities.Where(i => i.Type == ("city")).FirstOrDefault() == null)
            {
                await context.PostAsync("איפה?");

                context.Wait(this.MessageReceived);

            }
            else
                using (var client = new HttpClient())
                {
                    var escapedLocation = Regex.Replace(result.Entities.Where(i => i.Type == ("city")).FirstOrDefault().Entity.ToString(), @"\W+", "_");

                    dynamic response = JObject.Parse(await client.GetStringAsync($"http://api.wunderground.com/api/1910fe7aa3da5f4f/conditions/q/" + escapedLocation + ".json"));

                    dynamic observation = response.current_observation;
                    dynamic results = response.response.results;

                    if (observation != null)
                    {
                        string displayLocation = observation.display_location?.full;
                        decimal tempC = observation.temp_c;
                        string weather = observation.weather;

                        string outputEng = $"It is {weather} and {tempC} degrees in {displayLocation}.";
                        await context.PostAsync(await Services.TranslatorService.TranslateText(outputEng,"he"));
                    }
                    else if (results != null)
                    {
                        string outputEng = $"There is more than one '{result.Entities.Where(i => i.Type == ("city")).FirstOrDefault().ToString()}'. Can you be more specific?";
                        await context.PostAsync(await Services.TranslatorService.TranslateText(outputEng, "he"));                       
                    }


                }
        }
        private string GetGreeting()
        {
            var greetings = new List<string> { "שלום", "מה שלומך?", "היי", "מה אפשר לעזור?", " לשירותך, מה אפשר לעזור Bot-IT" };
            if (DateTime.Now.Hour < 12)
            {
                greetings.Add("בוקר טוב, מה אפשר לעזור?");
            }
            if (DateTime.Now.Hour > 11 && DateTime.Now.Hour < 12)
            {
                greetings.Add("בוקר טוב, בדרך לשולץ? מה אפשר לעזור?");
            }
            if (DateTime.Now.Hour > 13 && DateTime.Now.Hour < 14)
            {
                greetings.Add("קשה אחרי חדר האוכל הא? מה אוכל להועיל?");
            }
            if (DateTime.Now.Hour < 12)
            {
                greetings.Add("בוקר טוב!");
            }
            if (DateTime.Now.Hour > 15 && DateTime.Now.Hour < 17)
            {
                greetings.Add("אחר צהריים נעימים, מה אוכל לעזור?");

            }
            if (DateTime.Now.Hour > 17 && DateTime.Now.Hour < 20)
            {
                greetings.Add("ערב נעים, צריך משהו?");

            }
            if (DateTime.Now.Hour > 20 && DateTime.Now.Hour < 23)
            {
                greetings.Add("לילה טוב, מה אפשר לעזור?");

            }

            Random randomizer = new Random();
            int index = randomizer.Next(greetings.Count);
            string greeting = greetings[index];

            return greeting;

        }

        private async Task<string> GetCurrentWeather(string location)
        {
            using (var client = new HttpClient())
            {
                var escapedLocation = Regex.Replace(location, @"\W+", "_");

                dynamic response = JObject.Parse(await client.GetStringAsync($"http://api.wunderground.com/api/<key>/conditions/q/{escapedLocation}.json"));

                dynamic observation = response.current_observation;
                dynamic results = response.response.results;

                if (observation != null)
                {
                    string displayLocation = observation.display_location?.full;
                    decimal tempC = observation.temp_c;
                    string weather = observation.weather;
                    //var accessToken = await GetAuthenticationToken(ApiKey);
                    //var output = await TranslateText(input, targetLang, accessToken);
                    return $"It is {weather} and {tempC} degrees in {displayLocation}.";
                }
                else if (results != null)
                {
                    return $" לא הצלחתי להבין מה המיקום הזה, תוכלו לדייק?'{location}' ";
                }

                return null;
            }
        }


        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(":-) כרגע אפשר לאשר\\לדחות השתלמות או לפתוח תקלה ב2000. בקרוב עוד אופציות ");

            context.Wait(this.MessageReceived);
        }


    }
}
