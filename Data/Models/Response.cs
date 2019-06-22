using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParseCSVFromJson.Data.Models
{
    public class Response
    {
        public string task { get; set; }
        public int quantity { get; set; }
        public string description { get; set; }
        public string lineItems { get; set; }
    }
}
