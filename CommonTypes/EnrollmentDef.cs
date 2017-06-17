using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static LuisBot.CommonTypes.CommonTypes;

namespace LuisBot.CommonTypes
{
    public class EnrollmentDef
    {

        // public List<ActionItem> Definitions { get => _courseDefinitions; set => _courseDefinitions = value; }

        //  private List<ActionItem> _courseDefinitions;// = new List<ActionItem>();

        //public CourseDef()
        //{


        //}

        public static List<ActionItem> GetDefinitions()
        {
            return new List<ActionItem>
            {
                 new ActionItem()
                {
                    //ActionItemMask = ActionItemMask.None,
                    ActionToTake =  ActionToTake.Approve,//"ApproveCourseEnrollment",
                    MethodName = "ApproveCourseEnrollment",
                    ParametersList = new List<ActionParameters>
                    {
                        new ActionParameters { ParameterName = "EnrollmentRequestID", ParameterType = "Int" }
                    },
                    SystemName = "CoursesEnrollmentSystem",
                    WsUrl = "" //TODO: Replace with url

                },

                 new ActionItem()
                {
                    //ActionItemMask = ActionItemMask.None,
                    ActionToTake = ActionToTake.Reject,//"RejectCourseEnrollment",
                    MethodName = "RejectEnrollment",
                    ParametersList = new List<ActionParameters>
                    {
                        new ActionParameters { ParameterName = "EnrollmentRequestID", ParameterType = "Int" }
                    },
                    SystemName = "CoursesEnrollmentSystem",
                    WsUrl = "" //TODO: Replace with url

                },
                     new ActionItem()
                {
                   // ActionItemMask = ActionItemMask.None,
                    ActionToTake = ActionToTake.Enquire,//"ReConfirmCourseIdToEnroll",
                    MethodName = "GetCourseDetailsByID",
                    ParametersList = new List<ActionParameters>
                    {
                        new ActionParameters { ParameterName = "EnrollmentRequestID", ParameterType = "Int" }
                    },
                    SystemName = "CoursesEnrollmentSystem",
                    WsUrl = "" //TODO: Replace with url

                }

            };

        }
    }
}