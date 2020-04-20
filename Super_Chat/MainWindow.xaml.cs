using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
using System.Windows.Threading;

namespace Super_Chat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


        public MainWindow()
        {
            InitializeComponent();
        }

        private async void connect_button_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => socket.Connect("127.0.0.1", 8080));//подключаемся к серверному сокету
        }

        private async void send_Button_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => sendMessage());
        }

        void sendMessage()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
               (ThreadStart)delegate ()
               {
                   string message = messageTB.Text;//получаем текст сообщения
                   byte[] buffer = Encoding.ASCII.GetBytes(message);//кодируем наше сообщение для отправки
                   socket.Send(buffer);

                   //ответ сервера
                   byte[] answer = new byte[1024];
                   socket.Receive(answer);
                   chatBoxTB.Text += Encoding.ASCII.GetString(answer);

                   // закрываем сокет
                   // socket.Shutdown(SocketShutdown.Both);
                   // socket.Close();
               }
            );

        }

        private void messageTB_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            messageTB.Text = "";
        }
    }
}
