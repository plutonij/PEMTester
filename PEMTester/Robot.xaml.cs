using Newtonsoft.Json;
using PEMTester.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Threading;
using System.Text;

namespace PEMTester
{
    /// <summary>
    /// Interaction logic for Robot.xaml
    /// </summary>
    public partial class Robot : Window, INotifyPropertyChanged
    {
        private List<Post> loopPost;
        public string Commands { get; set; }
        public string MuxCommands { get; set; }
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
        private int executedRequests;
        public string ExecutedRequests
        {
            get
            {
                return string.Format("{0} requests executed", executedRequests);
            }
            set
            {
                if (int.TryParse(value, out executedRequests))
                {
                    NotifyPropertyChanged("ExecutedRequests");
                }
            }
        }

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

        private StringBuilder log;
        public string Log
        {
            get { return log.ToString();}
            set { log.AppendLine(value); NotifyPropertyChanged("Log"); }
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
            log = new StringBuilder();
        }

        private void OnRequestComplete(int result, string resultJson)
        {
            numberRequests--;
            NumberOfRequests = string.Format("running {0} requests", numberRequests);

            //Log = string.Format("<-- {0}{1}{0}", Environment.NewLine, resultJson);

            if (0 < loopPost.Count)
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
                string[] commandArray = Commands.Split(',');
                List<string> robotCommand = new List<string>();

                foreach(var command in commandArray)
                {
                    int result = 0;
                    if (int.TryParse(command, out result))
                    {
                        robotCommand.Add(string.Format("PRESS {0}", result));
                    }
                    else switch(command.ToLower())
                        {
                            case "insert":
                                robotCommand.Add("INSERT CARD");
                                break;
                            case "eject":
                                robotCommand.Add("REMOVE CARD");
                                break;
                            case "swipel":
                                robotCommand.Add("SWIPE L");
                                break;
                            case "swiper":
                                robotCommand.Add("SWIPE R");
                                break;
                            case "home":
                                robotCommand.Add("HOME");
                                break;

                            
                        }
                }



            Post commands = new Post()
                {
                    id = Id,
                    commands= robotCommand
                   
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

            //Log = string.Format("--> {0}{1}{0}", Environment.NewLine, JsonConvert.SerializeObject(p, Formatting.Indented));

            sender.Post(jsonString, url);

            ExecutedRequests = (++ executedRequests).ToString();
            

        }

        private void PinpadButtonClicked(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            string content = b.Content.ToString().ToUpper();

            if (b.Content.Equals("."))
            {
                content = "dot".ToUpper();
            }

            Post commands = new Post()
            {
                id = Id,
                commands = new List<string>() { string.Format("PRESS {0}", content) }
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

        private void ExecuteClickMux(object sender, RoutedEventArgs e)
        {

            if (MuxCommands != null)
            {
                foreach (var c in MuxCommands.Split(',').Select(x => string.Format("SELECT PORT {0}", x.ToUpper())).ToList())
                {
                    Post(CreateSingleCommand(c));
                    Thread.Sleep(1000);
                }
            }

        }

        private void ExecuteClickMag(object sender, RoutedEventArgs e)
        {
            if (MuxCommands != null)
            {
                foreach (var c in MuxCommands.Split(',').Select(x => string.Format("SELECT CARD {0}", x.ToUpper())).ToList())
                {
                    Post(CreateSingleCommand(c));
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
