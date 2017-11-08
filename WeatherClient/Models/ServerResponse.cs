using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherClient.Models
{
 
    public class Field
    {
        public string city;
        public long? date;
        public decimal? pressure;
        public decimal? humidity;
        public decimal? temp;
    }

    class ServerResponse
    {
        public string Command { get; set; }
        public List<Field> Object;
        public List<object> Error;
        public string city;
        public long date;
        public decimal pressure;
        public decimal humidity;
        public decimal temp;


        public ServerResponse()
        {
        }

        public ServerResponse(string jsonMessage)
        {

            dynamic obj = JsonConvert.DeserializeObject<ServerResponse>(jsonMessage);
            this.Command = obj.Command;
            this.Object = obj.Object;
            this.Error = obj.Error;

            city = this.Object.Where(p => p.city != null).Select(p => p.city).FirstOrDefault();
            date = this.Object.Where(p => p.date != null).Select(p => (long)p.date).FirstOrDefault();
            pressure = this.Object.Where(p => p.pressure != null).Select(p => (decimal)p.pressure).FirstOrDefault();
            humidity = this.Object.Where(p => p.humidity != null).Select(p => (decimal)p.humidity).FirstOrDefault();
            temp = this.Object.Where(p => p.temp != null).Select(p => (decimal)p.temp).FirstOrDefault();
        }

     
    }







}
