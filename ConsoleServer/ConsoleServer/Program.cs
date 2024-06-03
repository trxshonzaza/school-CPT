using System.Net.Sockets;
using System.Net;
using System.Text;

public class Program
{
    static TcpListener server = null;
    static AsyncCallback recv = null;

    static List<TcpClient> connected = new List<TcpClient>();

    public static void Main(string[] args)
    {
        server = new TcpListener(IPAddress.Parse("127.0.0.1"), 1000);

        server.Start();

        // this thread accepts TCP connections!

        // it is HIGHLY reccomended you separate your functions in threads
        // so that not everything is running on the main thread,
        // therefore everything can run asyncrohously.

        Thread conectionThread = new Thread(() =>
        {
            server.BeginAcceptTcpClient(recv = (ar) =>
            {
                Console.WriteLine("client connected...");

                TcpClient client = server.EndAcceptTcpClient(ar);
                server.BeginAcceptTcpClient(recv, ar.AsyncState);

                connected.Add(client);

                // now that the connection is added, you can send a receive data using NetworkStream.

            }, null);
        });

        // you must recieve data endlessly in a while loop.

        Thread receiveThread = new Thread(() =>
        {
            while (true)
            {
                try
                {
                    foreach (TcpClient client in connected)
                    {
                        while (client.GetStream().DataAvailable)
                        {
                            Console.WriteLine("there is data to be read...");

                            // read whatever data you need here...
                            // lets say we want to read the message that the client sent us..

                            byte[] buffer = new byte[client.ReceiveBufferSize];

                            int count = client.GetStream().Read(buffer, 0, buffer.Length);

                            string message = Encoding.UTF8.GetString(buffer, 0, count);

                            Console.WriteLine(message);

                            // now that our data is read, lets send some back..

                            byte[] toSend = Encoding.UTF8.GetBytes("hello back!");

                            client.GetStream().Write(toSend);
                        }
                    }
                }
                catch (Exception e) { }
            }
        });

        // make sure to start your threads at the end of your code!

        conectionThread.Start();
        receiveThread.Start();

        // you can stop your threads using "Thread.Abort()" which raises an exception
        // but terminates the thread entirely.

    }

}