using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuisBot
{

    public class Rootobject
    {
        public string luis_schema_version { get; set; }
        public string versionId { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public string culture { get; set; }
        public Intent[] intents { get; set; }
        public Entity[] entities { get; set; }
        public object[] composites { get; set; }
        public object[] closedLists { get; set; }
        public string[] bing_entities { get; set; }
        public object[] actions { get; set; }
        public object[] model_features { get; set; }
        public object[] regex_features { get; set; }
        public Utterance[] utterances { get; set; }
    }

    public class Intent
    {
        public string name { get; set; }
    }

    public class Entity
    {
        public string name { get; set; }
    }

    public class Utterance
    {
        public string text { get; set; }
        public string intent { get; set; }
        public Entity1[] entities { get; set; }
    }

    public class Entity1
    {
        public string entity { get; set; }
        public int startPos { get; set; }
        public int endPos { get; set; }
    }


}