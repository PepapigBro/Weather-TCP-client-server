using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherClient.Models
{
  
   public class PostRequest
    {
        public string Command { get; set; }
        
        public List<object> Params;

        public PostRequest() {
            
           Params = new List<object>();
        }

    }

   
   
}
