﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        public static List<Socket> clients = new List<Socket>();
        public static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static int port = 8080; // порт для приема входящих запросов
        public static IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
        public static byte[] userName = new byte[64];
        public static Socket client;
        static void Main(string[] args)
        {
            try
            {
                Task.Run(() => serv());
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void serv()
        {
            try
            {
               
                socket.Bind(endPoint);//привязываем сервер к определенному адресу
                socket.Listen(15);//задаем количество одновременных подключений и ставим сокет в режим прослушивания
                Console.WriteLine("Сервер запущен. Ожидание подключений...");
                while (true)
                {
                    client = socket.Accept();//принимаем нового клиента
                    clients.Add(client);
                    client.Receive(userName);

                    Console.WriteLine($"Новый пользователь - {Encoding.UTF8.GetString(userName)}");
                    foreach (var something in clients)
                    {
                        something.Send(Encoding.UTF8.GetBytes($"Новый пользователь - {Encoding.UTF8.GetString(userName)}"));
                    }
                    Task.Run(() => userMessage(client));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void userMessage(Socket some)
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];//данные от пользователя
                    int size = 0;
                    StringBuilder builder = new StringBuilder();

                    do
                    {
                        size = some.Receive(buffer);//получаем данные от пользователя
                        builder.Append(Encoding.UTF8.GetString(buffer, 0, size));
                    }
                    while (some.Available > 0);

                    Console.WriteLine(builder);
                    foreach (var something in clients)
                    {
                        something.Send(buffer);
                    }
                }
            }
            catch (Exception ex)
            {
                foreach (var something in clients)
                {
                    something.Send(Encoding.UTF8.GetBytes($"{Encoding.UTF8.GetString(userName)} покинул чат."));
                }
                socket.Shutdown(SocketShutdown.Both);//выключили
                clients.Remove(client);
                //socket.Close();//закрыли
                throw ex;
            }

        }
    }
}
