using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherService.Models
{
    public static class Setting
    {
       public static string PathToLocalDB = "e:\\Databases\\WhetherDB.sqlite";
       public static int Port = 7777;
       public static string Ip = "127.0.0.1";   
       public static string ApiKey = "5317abebcc52f597139f6b0fe1c5d3ea";

        public static Dictionary<int, string> Cities = new Dictionary<int, string> { { 2643741, "City of London" }, { 524901, "Moscow" }, { 5391959, "San Francisco" }, { 1816670, "Beijing" } };

    }
}
