using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotFramework.Entities
{
    public class OrderMessage
    {
        public string MesssageId { get; set; }
        public string OrderId { get; set; }
        public string company { get; set; }
        public string date { get; set; }
        public string applicant { get; set; }
        public string SubTotal { get; set; }
        public string Tax { get; set; }
        public string Total { get; set; }

    }
}
