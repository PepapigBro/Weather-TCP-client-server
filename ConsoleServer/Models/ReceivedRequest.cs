using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleServer.Models
{
    public class Field
    {
        public string city;
        public long? date;
    }
    
    class ReceivedRequest
    {
        public string Command { get; set; }
        public List<Field> Params;
        public string city;
        public long date;

        public ReceivedRequest()
        {     
        }

        public ReceivedRequest(string jsonMessage)
        {

            dynamic obj = JsonConvert.DeserializeObject<ReceivedRequest>(jsonMessage);
            this.Command = obj.Command;
            this.Params = obj.Params;

            city = this.Params.Where(p => p.city != null).Select(p => p.city).FirstOrDefault();
            date = this.Params.Where(p => p.date != null).Select(p => (long)p.date).FirstOrDefault();
        }
    }


}
