using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherService.Models
{
    class ServerResponse
    {
        public string Command { get; set; }
        public List<object> Error { get; set; }
        public List<object> Object { get; set; }

        public ServerResponse() {
            Error = new List<object>();
            Object = new List<object>();
        }

        
    }
}
