
using WeatherService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WeatherService.Models
{
    public class ClientObject
    {
        protected internal TcpClient client { get; private set; }
        protected internal NetworkStream stream { get; private set; }
        ServerObject server;
        

        private static long convertDateTimeToTimeSpan(DateTime datetime)
        {            
            //convert using utc
            DateTime unixStart = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
            long epoch = (long)Math.Floor((datetime.ToUniversalTime() - unixStart).TotalSeconds);
            return epoch;
        }

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            client = tcpClient;
            server = serverObject;
        }

        public Forecast GetDataFromDB(string city, long timestamp) {

            Forecast forecast = null;
            using (DatabaseContext db = new DatabaseContext())
            {
                //get forecast  for the near earlier date specified by the user
                forecast = db.Forecasts.Include("City").Where(p => p.City.Name == city && p.Time<= timestamp).OrderByDescending(p=>p.Time).FirstOrDefault();

                if (forecast == null)
                {
                    //try find near date later selected
                    forecast = db.Forecasts.Include("City").Where(p => p.City.Name == city && p.Time >= timestamp).OrderBy(p => p.Time).FirstOrDefault();
                }
            }
            return forecast;
        }

        // convert received data to string
        private string GetMessage()
        {
            byte[] data = new byte[4]; // buffer for received data
            
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);

            return builder.ToString();
        }

        // close connection
        protected internal void Close()
        {
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();
        }

        //runs in a separate thread when client is connect
        public void Process()
        {
            try
            {
                stream = client.GetStream();
               
                // wait client messages
                while (true)
                {
                    //get message
                    string message = GetMessage();
                    //Convert to obj
                    ReceivedRequest receivedRequest = new ReceivedRequest(message);

                    //get data from db
                    Forecast forecast = null;
                    if (receivedRequest.Command == "GetWeather")
                    {
                        forecast = GetDataFromDB(receivedRequest.city,(long)receivedRequest.date);
                    }

                    //send response to cliend
                    server.SendMessage(forecast);
                }

            }


            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                Close();
            }

            
         }
       }
}
 

