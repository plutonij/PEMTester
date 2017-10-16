using Newtonsoft.Json;
using PEMTester.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace PEMTester
{
    /// <summary>
    /// Interaction logic for Robot.xaml
    /// </summary>
    public partial class Robot : Window, INotifyPropertyChanged
    {
        private List<Post> loopPost;
        public string Commands { get; set; }
        private string id;
        private string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                NotifyPropertyChanged("Id");
            }
        }
        private string url;
        private int numberRequests = 0;

        private string numberOfRequests;
        public string NumberOfRequests
        {
            get
            {
                return numberOfRequests;
            }
            set
            {
                numberOfRequests = value;
                NotifyPropertyChanged("NumberOfRequests");
            }
            

        }

        private bool? loop;
        public bool? Loop
        {
            get
            {
                return loop?? false;
            }
            set
            {
                loop = value;
                NotifyPropertyChanged("Loop");
            }
        }

        private HttpSender sender;


        public Robot(string id, string url)
        {
            
            InitializeComponent();
            DataContext = this;
            sender = HttpSender.Instance;
            sender.OnPostComplete += OnRequestComplete;
            Id = id;
            this.url = url;
            Title = id;
            numberOfRequests = string.Format("running {0} requests", numberRequests);
            loopPost = new List<Post>();
        }

        private void OnRequestComplete(int result)
        {
            numberRequests--;
            NumberOfRequests = string.Format("running {0} requests", numberRequests);

            if(0 < loopPost.Count)
            {
                var command = loopPost[0];

                if (loop == false)
                {
                    loopPost.RemoveAt(0);
                }

                Post(command);
            }

            //todo result
        }

        private void ExecuteClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] command = Commands.Split(',');

                Post commands = new Post()
                {
                    id = Id,
                    commands = Commands.Split(',')
                                       .Select(x => string.Format("PRESS {0}", x.ToUpper()))
                                       .ToList()
                };

                Post(commands);

                if(true == loop)
                {
                    loopPost.Add(commands);
                }
            }
            catch
            {
                NumberOfRequests = string.Format("The request field is empty");
            }
        }

        private void Post(Post p)
        {
           string jsonString = JsonConvert.SerializeObject(p);

            numberRequests++;
            NumberOfRequests = string.Format("running {0} requests", numberRequests);
            sender.Post(jsonString, url);
        }

        private void PinpadButtonClicked(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;

            Post commands = new Post()
            {
                id = Id,
                commands = new List<string>() { string.Format("PRESS {0}", b.Content.ToString().ToUpper()) }
            };

            Post(commands);

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void HomeClick(object sender, RoutedEventArgs e)
        {
            var commands = CreateSingleCommand("Home".ToUpper());
            Post(commands);
        }

        private void CardClick(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            var commands = CreateSingleCommand(string.Format("{0} CARD", b.Content.ToString()).ToUpper());
            Post(commands);
        }

        private void SleepClick(object sender, RoutedEventArgs e)
        {
            Post(CreateSingleCommand("sleep".ToUpper()));
        }

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            //Post(CreateSingleCommand("sleep".ToUpper()));
        }

        private Post CreateSingleCommand(string command)
        {
            return new Post()
            {
                id = Id,
                commands = new List<string>() { command }
            };
        }
    }
}
