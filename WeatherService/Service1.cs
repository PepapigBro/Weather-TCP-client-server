using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeatherService.Models;

namespace WeatherService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();

            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }


        static ServerObject server; // server obj
        static Thread listenThread; // thread for listener
        protected override void OnStart(string[] args)
        {
                 //start the scheduler receiving from the external server and saving in local database of weather on schedule

                UpdateSheduler sheduler = new UpdateSheduler(minutesInterval: 60, cities: Setting.Cities);
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

        protected override void OnStop()
        {
            server.Disconnect();
        }
    }
}
