using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder
{
    public class messageMarker
    {
        public ulong messageId { get; set; }
        public string adventureName { get; set; }
        public string segIndex { get; set; }

        public messageMarker(ulong messageId, string adventureName, string segIndex)
        {
            this.messageId = messageId;
            this.adventureName = adventureName;
            this.segIndex = segIndex;
        }
    }
}
