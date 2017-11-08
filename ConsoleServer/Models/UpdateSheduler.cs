using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using System.Threading.Tasks;
using System.Timers;

namespace ConsoleServer.Models
{
    class UpdateSheduler
    {
        private double interval;  //the refresh interval for data with an external server   
        Dictionary<int, string> availableCities;
        Timer checkForTime;
        

        private static long convertDateTimeToTimeSpan(DateTime datetime)
        {
            DateTime unixStart = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
            long epoch = (long)Math.Floor((datetime.ToUniversalTime() - unixStart).TotalSeconds);
            return epoch;
        }


        private UpdateSheduler() { }

        //constructor
        public UpdateSheduler(double minutesInterval, Dictionary<int, string> cities)
        {
            
            availableCities = cities;

            //Force run without waiting for the schedule when you first start
            checkForTime_Elapsed(null, null);

            this.availableCities = cities;

            interval = minutesInterval * 60 * 1000; // convert to ms                                                                  

            checkForTime = new Timer(interval);
            checkForTime.Elapsed += new ElapsedEventHandler(checkForTime_Elapsed);
            
        }

        //start sheduler
        public void Start()
        {
            checkForTime.Enabled = true;
        }

        
        private dynamic getWeatherByAPI(int id)
        {
            string currentURL = "http://api.openweathermap.org/data/2.5/weather?id=" + id + "&APPID=" + Setting.ApiKey;
            string content;
            dynamic jsonObj;

            using (WebClient client = new WebClient())
            {
                content = client.DownloadString(currentURL);
                jsonObj = JsonConvert.DeserializeObject(content);
            }
            return jsonObj;
        }

        private void saveToDB(dynamic weather) {

            using (DatabaseContext db = new DatabaseContext())
            {
                string cityname = (string)weather.name;
                City city = db.Cities.Where(n => n.Name == cityname).SingleOrDefault();

                //add city to DB if not exist
                if (city==null)
                {
                    city = new City() { Name = cityname, Country = weather.sys.country , Weather_api_id = weather.id };
                    db.Cities.Add(city);
                    db.SaveChanges();
                    Console.WriteLine("city success");
                }
                 //add current forecast to DB
                  Forecast forecast = new Forecast() { City_Id = city.Id, Humidity = weather.main.humidity, Pressure = weather.main.pressure, Temp = weather.main.temp, Time = convertDateTimeToTimeSpan(DateTime.UtcNow) };
                  db.Forecasts.Add(forecast);
                  db.SaveChangesAsync();
                Console.WriteLine("forecast success");

            }
            
        }

        //method called by interval
        private void checkForTime_Elapsed(object sender, ElapsedEventArgs e) {

            
             //get data for each city
             foreach (var id_name in availableCities)
            {
                Task task = new Task(() => {
                    dynamic weather = getWeatherByAPI(id_name.Key);
                    saveToDB(weather);

               } );
               task.Start();
                

            }
            
        }

      

    }
}
