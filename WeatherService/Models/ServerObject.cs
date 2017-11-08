
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace WeatherService.Models
{
    public class ServerObject
    {
        static TcpListener tcpListener; 
        ClientObject clientObject;   

        public ServerObject()            
        {
        }
            
        // listening incoming
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Parse(Setting.Ip), Setting.Port);
                tcpListener.Start();
                Console.WriteLine("Connect. Wait client...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    //create new thread for every new client and listen
                    clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        // send the message to client
        protected internal void SendMessage(Forecast forecast)
        {
            string jsonString = "";
            ServerResponse serverResponse = new ServerResponse();
            if (forecast != null)
            {
                
                serverResponse.Command = "GetWeather";
                //serverResponse.Error=
                serverResponse.Object.Add(new { city = forecast.City.Name });
                serverResponse.Object.Add(new { date = forecast.Time });
                serverResponse.Object.Add(new { pressure = forecast.Pressure });
                serverResponse.Object.Add(new { humidity = forecast.Humidity });
                serverResponse.Object.Add(new { temp = forecast.Temp });
            }
            else
            {
                serverResponse.Command = "GetWeather";
                serverResponse.Error.Add(new { message = "Error get forecast" });
            }
            
            jsonString = JsonConvert.SerializeObject(serverResponse);

            byte[] data = Encoding.Unicode.GetBytes(jsonString);
            data = Encoding.Unicode.GetBytes(jsonString);
            clientObject.stream.Write(data, 0, data.Length);
        }



        
        protected internal void Disconnect()
        {
            tcpListener.Stop(); //stop server
            clientObject.Close(); //stop client
            
          //  Environment.Exit(0); //end process
        }
    }
}