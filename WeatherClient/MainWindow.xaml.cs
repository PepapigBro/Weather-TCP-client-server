using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WeatherClient.Models;

namespace WeatherClient
{
   

    public partial class MainWindow : Window
    {

        static TcpClient client = null;
        static NetworkStream stream;

        public event Action eventConnect;
        public event Action eventDisconnect;
        private List<string> availableCities;

        public MainWindow()
        {
            InitializeComponent();

            availableCities = new List<string>(new string[] { "City of London", "Moscow", "San Francisco", "Beijing" });
            foreach(string city in availableCities)
            {
                comboBox.Items.Add(city);
            }
            
            comboBox.SelectedIndex = 0;

            eventConnect += isConnect;
            eventDisconnect += isDisconnect;

            eventDisconnect();

            selectedDateTime.FormatString = "dd.MM.yyyy HH:mm";
            selectedDateTime.Value = DateTime.Now;        

        }

        private void isDisconnect()
        {
            try //if main window close throw exception
            {
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    labelState.Foreground = Brushes.DarkRed;
                    labelState.Content = "connection not established";
                    buttonGetWeather.IsEnabled = false;
                    buttonDisconnect.IsEnabled = false;
                    buttonConnect.IsEnabled = true;
                }));
            }
            catch { }

            
        }

        private void isConnect()
        {
            
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                labelState.Foreground = Brushes.DarkGreen;
                labelState.Content = "connection established";
                labelError.Content = "";
                buttonGetWeather.IsEnabled = true;
                buttonConnect.IsEnabled = false;
                buttonDisconnect.IsEnabled = true;

            }));

        }

        private void numberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
            if ((sender as TextBox).Text.Length >= 5) { e.Handled = true; }
        }

        private bool tryConvertIP(ref string text)
        {

            IPAddress ip;
             bool isValid= IPAddress.TryParse(text, out ip);
            if (ip != null) {
                text = ip.ToString();
            }
            return isValid;

        }

        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            //validation ip
            string ip = textBoxIP.Text;
            bool isValid =  tryConvertIP(ref ip);
            textBoxIP.Text = ip;
            if (!isValid)
            {
                labelValidationIP.Foreground = Brushes.Red;
                labelValidationIP.Content = "Not valid";
                return;
            }
            else
            {
                labelValidationIP.Foreground = Brushes.Green;
                labelValidationIP.Content = "valid";
            }

           
            int port = Convert.ToInt32(textBoxPort.Text);

            
            Task task = new Task(() => {
                        try
                        {
                            client = new TcpClient();
                            client.Connect(ip, port); //connect of client
                            stream = client.GetStream();

                            // start new thread to listen for incoming messages by loop
                            Thread receiveThread = new Thread(new ThreadStart(ReceivingMessage));
                            receiveThread.Start(); //старт потока

                            eventConnect();
                        }
                        catch
                        {
                            Disconnect();
                        }
                     });
            
            task.Start();

        }

       
        
        private void buttonGetWeather_Click(object sender, RoutedEventArgs e)
        {
            if (client == null) { return; }


            //The receipt date for required forecast. If the date is not filled in, set the current day and time
            DateTime? selectedDate = selectedDateTime.Value;
            if (selectedDate == null) {
                selectedDate = DateTime.Now;
                selectedDateTime.Value = selectedDate;
            }

            //Create request object to send
            PostRequest postRequest = new PostRequest();
            postRequest.Command = "GetWeather";
            postRequest.Params.Add(new { city = comboBox.SelectedValue });
            postRequest.Params.Add(new { date = convertDateTimeToTimeSpan((DateTime)selectedDate)});

            string jsonString = JsonConvert.SerializeObject(postRequest);

            //Send to server
            try
            {
                Task task = new Task(() => {
                    SendMessage(jsonString);
                });
                task.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
               // Disconnect();
            }
        }

         void UpdateUi(ServerResponse serverResponse)
        {

            //Displaying data in a wpf window
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                labelLatestForecast.Content = UnixTimeStampToDateTime(serverResponse.date).ToString("dd.MM.yyyy hh:mm"); ;
                labelPressure.Content = serverResponse.pressure;
                labelHumidity.Content = serverResponse.humidity;
                labelCity.Content = serverResponse.city;
                labelTemperature.Content = Math.Round(serverResponse.temp - (decimal)273.15, 1) + " *C";  //Convert Keliv to Celcium 
            }));

                      

        }


        //  send message to server
        void SendMessage(string message)
        {        
                byte[] data = Encoding.Unicode.GetBytes(message);               
                stream.Write(data, 0, data.Length);
        }

        // receiving messages
         void ReceivingMessage()
        {
            while (stream != null && client != null)
            {
                try
                {
                    byte[] data = new byte[4]; //buffer for received data

                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);


                    string message = builder.ToString();

                    ServerResponse serverResponse = new ServerResponse(message);
                    UpdateUi(serverResponse);


                }
                catch
                {
                    Console.WriteLine("Break COnnect!");
                    Disconnect();
                }
            }
        }




        private void buttonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();            
        }


        void Disconnect()
        {
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();

            stream = null;
            client = null;
            eventDisconnect();
           
        }

        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime.ToLocalTime();
        }
        private static long convertDateTimeToTimeSpan(DateTime datetime)
        {
            DateTime unixStart = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
            long epoch = (long)Math.Floor((datetime.ToUniversalTime() - unixStart).TotalSeconds);
            return epoch;
        }
    }
}
