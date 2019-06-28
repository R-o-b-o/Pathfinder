using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder
{
    public class AdventureConfig
    {
        public string name { get; set; }
        public string description { get; set; }
        public string creator { get; set; }
        public string imageurl { get; set; }
        public int plays { get; set; }
    }
    public class AdventureSegment
    {
        public string maintext { get; set; }
        public string choicetext { get; set; }
        public AdventureChoice choice { get; set; }
    }
    public class AdventureChoice
    {
        public string emote { get; set; }
        public string text { get; set; }
        public string target { get; set; }
    }
    public class Adventure
    {
        public AdventureConfig config { get; set; }
        public Dictionary<string, AdventureSegment> segments { get; set; }
    }
}
