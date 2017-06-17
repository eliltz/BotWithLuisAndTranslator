using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static LuisBot.CommonTypes.CommonTypes;

namespace LuisBot.CommonTypes
{
  
    public class CommonTypes
    {
        [Flags]
        public enum ActionToTake
        {
            None = 0,
            Initiate = 1,
            Approve = 2,
            Reject = 4,
            Enquire = 8
        }

    }

    [Flags]
    [Serializable]
    public enum ActionItemMask
    {
        None = 0,
        SystemName = 1,
        ActionToTake = 2,
        FullParamsList = 4//ItemId = 4 //TODO: Maybe change this to IsParametersFull
    }

    [Serializable]
    public class ActionItem
    {
        public string SystemName { get; set; }
        public string WsUrl { get; set; }
        public ActionToTake ActionToTake { get; set; }
        //List<string> actionsTagsList = new List<string>("אישור", "", "");//ReadFromFile
        public string MethodName { get; set; }
        public List<ActionParameter> ParametersList { get; set; }
        public ActionItemMask ActionItemMask { get; set; }

    }

    [Serializable]
    public class ActionParameter
    {
        public string ParameterName { get; set; }
        public string ParameterType { get; set; }
        public string ParameterValue { get; set; }
    }
}