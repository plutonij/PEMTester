using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Windows;
using Newtonsoft.Json;
using PEMTester.Objects;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;

namespace PEMTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<Robot> Robots;
        private ObservableCollection<string> ids;

        public ObservableCollection<string> Ids
        {
            get
            {
                return ids;
            }
            set
            {
                ids = value;
                NotifyPropertyChanged("Ids");
            }
        }
        public string Port { get; set; }
        public string IP { get; set; }
            
        public MainWindow()
        {
          
            InitializeComponent();
            DataContext = this;
            Port = "8000";
            IP = "localhost";
            Robots = new List<Robot>();
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string json = string.Empty;
            string url = string.Format("http://{0}:{1}/", IP, Port);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
           // request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }

            GetId tmp = JsonConvert.DeserializeObject<GetId>(json);
            Ids = new ObservableCollection<string>(tmp.id);
            Console.WriteLine(json);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void OnListItemClick(object sender, RoutedEventArgs e)
        {
            var item = sender as ListBoxItem;
            string id = item.Content as string;

            Robot r = new Robot(id, string.Format("http://{0}:{1}/", IP, Port));
            r.Show();
        }
    }
}
