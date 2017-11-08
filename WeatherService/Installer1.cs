﻿

using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;
 
namespace WeatherService
{
    [RunInstaller(true)]
    public partial class Installer1 : Installer
    {
        ServiceInstaller serviceInstaller;
        ServiceProcessInstaller processInstaller;

        public Installer1()
        {
            InitializeComponent();
            serviceInstaller = new ServiceInstaller();
            processInstaller = new ServiceProcessInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = "WeatherService";
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}