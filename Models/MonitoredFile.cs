using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGProToCCAddressHelper.Models
{
    internal class MonitoredFile
    {
        public string fullName { get; set; } = string.Empty;
        public long size { get; set; } = 0;
        public DateTime monitoredTime { get; set; } = DateTime.MinValue;
    }
}
