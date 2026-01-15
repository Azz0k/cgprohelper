using System;
using System.Collections.Generic;
using System.Text;

namespace GateKeeper.Helper.Models
{
    public class EmailFields
    {
        public string From { get; set; }
        public List<string> To { get; set; } = [];
        public EmailFields(string from) 
        {
            From = from;
        }
    }
}
