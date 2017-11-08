
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Threading;
using ConsoleServer.Models;

namespace ConsoleServer
{
    class Program
    {       
           
        static ServerObject server; // server obj
        static Thread listenThread; // thread for listener

        static void Main(string[] args)
        {
            //start the scheduler receiving from the external server and saving in local database of weather on schedule

           UpdateSheduler sheduler = new UpdateSheduler(minutesInterval: 1, cities: Setting.Cities);
           sheduler.Start();
         
           try
           {
               server = new ServerObject();
         
               //create listener
               listenThread = new Thread(new ThreadStart(server.Listen));
               listenThread.Start(); 
           }
           catch (Exception ex)
           {
               server.Disconnect();
               Console.WriteLine(ex.Message);
           }

        

        }
    }
}
