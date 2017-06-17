using System;
using Microsoft.Bot.Builder.FormFlow;

namespace LuisBot.FormFlow
{
   
        [Serializable]
        public class EnrollmentApprovalQuery
        {
            [Prompt("נא לכתוב את מס' בקשת ההשתלמות")]
            // [Optional]

            public string Destination { get; set; }

            //[Prompt("נא הקלד 'אשר' לאישור או 'דחה' לדחיית בקשת ההשתלמות")]
            //[Optional]
            //public string AirportCode { get; set; }
        }
   
}