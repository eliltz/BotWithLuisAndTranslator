﻿namespace LuisBot.Dialogs
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
            string message = $"ביקשת לאשר בקשת השתלמות, אנא המתן...";
            //if (result.Entities.Count == 0)
            //{
            //    await context.PostAsync("חסרים פרטים באישור בקשת ההשתלמות");
            //}
            // else
            if (result.Entities.Where(i => i.Type == ("EnrollmentRequestID")).FirstOrDefault() == null)
            {
                await context.PostAsync("חסרים פרטים באישור בקשת ההשתלמות");

                context.Wait(this.MessageReceived);
            }
            else
                context.Call<object>(new CoursesDialog(), AfterCourseDialogIsDone);


            // context.Wait(this.MessageReceived);
        }

        private async Task AfterCourseDialogIsDone(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("האם תרצה לבצע משהו נוסף?");
            // await context.PostAsync(message + "asdasd");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Reject")]
        public async Task RejectCourseEnrollmentIntent(IDialogContext context, LuisResult result)
        {
            string message = $"";

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
        [LuisIntent("Approve")]
        public async Task Approve(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            //var message = await activity;
            //await context.PostAsync($"Welcome to the Hotels finder! We are analyzing your message: '{message.Text}'...");

            //var hotelsQuery = new HotelsQuery();

            //EntityRecommendation cityEntityRecommendation;

            //if (result.TryFindEntity(EntityGeographyCity, out cityEntityRecommendation))
            //{
            //    cityEntityRecommendation.Type = "Destination";
            //}

            //var hotelsFormDialog = new FormDialog<HotelsQuery>(hotelsQuery, this.BuildHotelsForm, FormOptions.PromptInStart, result.Entities);

            //context.Call(hotelsFormDialog, this.ResumeAfterHotelsFormDialog);
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

                    dynamic response = JObject.Parse(await client.GetStringAsync($"http://api.wunderground.com/api/1910fe7aa3da5f4f/conditions/q/"+ escapedLocation+".json"));

                    dynamic observation = response.current_observation;
                    dynamic results = response.response.results;

                    if (observation != null)
                    {
                        string displayLocation = observation.display_location?.full;
                        decimal tempC = observation.temp_c;
                        string weather = observation.weather;

                        await context.PostAsync($"It is {weather} and {tempC} degrees in {displayLocation}.");
                    }
                    else if (results != null)
                    {
                        await context.PostAsync($"There is more than one '{result.Entities.Where(i => i.Type == ("city")).FirstOrDefault().ToString()}'. Can you be more specific?");
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

                    return $"It is {weather} and {tempC} degrees in {displayLocation}.";
                }
                else if (results != null)
                {
                    return $"There is more than one '{location}'. Can you be more specific?";
                }

                return null;
            }
        }


        //[LuisIntent("NONONONON")]
        //public async Task Reviews(IDialogContext context, LuisResult result)
        //{
        //    EntityRecommendation hotelEntityRecommendation;

        //    if (result.TryFindEntity(EntityHotelName, out hotelEntityRecommendation))
        //    {
        //        await context.PostAsync($"Looking for reviews of '{hotelEntityRecommendation.Entity}'...");

        //        var resultMessage = context.MakeMessage();
        //        resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
        //        resultMessage.Attachments = new List<Attachment>();

        //        for (int i = 0; i < 5; i++)
        //        {
        //            var random = new Random(i);
        //            ThumbnailCard thumbnailCard = new ThumbnailCard()
        //            {
        //                Title = this.titleOptions[random.Next(0, this.titleOptions.Count - 1)],
        //                Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris odio magna, sodales vel ligula sit amet, vulputate vehicula velit. Nulla quis consectetur neque, sed commodo metus.",
        //                Images = new List<CardImage>()
        //                {
        //                    new CardImage() { Url = "https://upload.wikimedia.org/wikipedia/en/e/ee/Unknown-person.gif" }
        //                },
        //            };

        //            resultMessage.Attachments.Add(thumbnailCard.ToAttachment());
        //        }

        //        await context.PostAsync(resultMessage);
        //    }

        //    context.Wait(this.MessageReceived);
        //}

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("אפשר למשל לאשר השתלמות, או לפתוח תקלה ב2000. בקרוב עוד אופציות:)");

            context.Wait(this.MessageReceived);
        }

        private IForm<HotelsQuery> BuildHotelsForm()
        {
            OnCompletionAsyncDelegate<HotelsQuery> processHotelsSearch = async (context, state) =>
            {
                var message = "Searching for hotels";
                if (!string.IsNullOrEmpty(state.Destination))
                {
                    message += $" in {state.Destination}...";
                }
                else if (!string.IsNullOrEmpty(state.AirportCode))
                {
                    message += $" near {state.AirportCode.ToUpperInvariant()} airport...";
                }

                await context.PostAsync(message);
            };

            return new FormBuilder<HotelsQuery>()
                .Field(nameof(HotelsQuery.Destination), (state) => string.IsNullOrEmpty(state.AirportCode))
                .Field(nameof(HotelsQuery.AirportCode), (state) => string.IsNullOrEmpty(state.Destination))
                .OnCompletion(processHotelsSearch)
                .Build();
        }

        private async Task ResumeAfterHotelsFormDialog(IDialogContext context, IAwaitable<HotelsQuery> result)
        {
            try
            {
                var searchQuery = await result;

                var hotels = await this.GetHotelsAsync(searchQuery);

                await context.PostAsync($"I found {hotels.Count()} hotels:");

                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();

                foreach (var hotel in hotels)
                {
                    HeroCard heroCard = new HeroCard()
                    {
                        Title = hotel.Name,
                        Subtitle = $"{hotel.Rating} starts. {hotel.NumberOfReviews} reviews. From ${hotel.PriceStarting} per night.",
                        Images = new List<CardImage>()
                        {
                            new CardImage() { Url = hotel.Image }
                        },
                        Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = "More details",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://www.bing.com/search?q=hotels+in+" + HttpUtility.UrlEncode(hotel.Location)
                            }
                        }
                    };

                    resultMessage.Attachments.Add(heroCard.ToAttachment());
                }

                await context.PostAsync(resultMessage);
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "You have canceled the operation.";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }

        private async Task<IEnumerable<Hotel>> GetHotelsAsync(HotelsQuery searchQuery)
        {
            var hotels = new List<Hotel>();

            // Filling the hotels results manually just for demo purposes
            for (int i = 1; i <= 5; i++)
            {
                var random = new Random(i);
                Hotel hotel = new Hotel()
                {
                    Name = $"{searchQuery.Destination ?? searchQuery.AirportCode} Hotel {i}",
                    Location = searchQuery.Destination ?? searchQuery.AirportCode,
                    Rating = random.Next(1, 5),
                    NumberOfReviews = random.Next(0, 5000),
                    PriceStarting = random.Next(80, 450),
                    Image = $"https://placeholdit.imgix.net/~text?txtsize=35&txt=Hotel+{i}&w=500&h=260"
                };

                hotels.Add(hotel);
            }

            hotels.Sort((h1, h2) => h1.PriceStarting.CompareTo(h2.PriceStarting));

            return hotels;
        }
    }
}
