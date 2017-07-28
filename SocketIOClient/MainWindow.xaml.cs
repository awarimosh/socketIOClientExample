using System;
using System.Windows;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace SocketIOClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket socket;
        private bool _connected;
        private Socket _instance;
        private bool addedUser = false;
        private User localUser;

        public class User
        {
            public User() { }
            public User(String u)
            {
                username = u;
            }
            public String username { get; set; }
        }

        public class Mess
        {
            public Mess() { }
            public Mess(String u, String m)
            {
                username = u;
                message = m;
            }
            public String username { get; set; }
            public String message { get; set; }
            public override string ToString()
            {
                return username + "  " + message;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            MainText.Text = "Mosh Socket Test #fuckAslamWatdog";
            //var options = CreateOptions();
            //var options = new IO.Options() { IgnoreServerCertificateValidation = true, AutoConnect = true, ForceNew = true };
            //options.Transports = new List<string>() { "websocket" };
            _instance = IO.Socket("ws://localhost:3000");
            _instance.On(Socket.EVENT_CONNECT, () =>
            {
                _connected = true;
                Console.WriteLine("Connected");
            });
            _instance.On(Socket.EVENT_DISCONNECT, () =>
            {
                _connected = false;
                Console.WriteLine("Disconnected");
            });
            _instance.On("new message", (data) =>
            {
                //Console.WriteLine(data);
                var jobject = data as JToken;

                // get the message data values
                var username = jobject.Value<string>("username");
                var message = jobject.Value<string>("message");
                Console.WriteLine(username + " : " + message);
                //MessageWindow = MainText.Text + "\r\n" + username + " : " + message;
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    // do whatever you want to do with shared object.
                }
                else
                {
                    //Other wise re-invoke the method with UI thread access
                    Application.Current.Dispatcher.Invoke(new System.Action(() => setMainWindow(username, message)));
                }
            });
        }

        private IO.Options CreateOptions()
        {
            var options = new IO.Options();
            options.Port = 3000;
            options.Hostname = "http://localhost";
            options.ForceNew = true;
            //log.Info("Please add to your hosts file: 127.0.0.1 " + options.Hostname);

            return options;
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if (!addedUser)
            {
                User user = new User("C#");
                localUser = user;
                string json1 = JsonConvert.SerializeObject(user);
                _instance.Emit("add user", json1);
                Console.WriteLine(json1);
                addedUser = !addedUser;
            }
            Mess mess = new Mess(localUser.username, EnterText.Text);
            string json = JsonConvert.SerializeObject(mess);
            Console.WriteLine(json);
            _instance.Emit("new message", json);
            EnterText.Text = "";
        }

        private void setMainWindow(String uName, String mess)
        {
            MainText.Text = MainText.Text + "\r\n" + uName + " : " + mess;
        }
    }
}
