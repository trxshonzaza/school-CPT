using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Security.Cryptography;

public class Program
{
    static TcpClient client = null;
    public static void Main(string[] args)
    {
        client = new TcpClient();

        client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1000));

        Thread.Sleep(5000);

        client.GetStream().Write(Encoding.UTF8.GetBytes("hello!"));

        // it is HIGHLY reccomended you separate your functions in threads
        // so that not everything is running on the main thread,
        // so everything can run asyncrohously.

        Thread receiveThread = new Thread(() =>
        {
            while (true)
            {
                if(client.GetStream().DataAvailable)
                {
                    Console.WriteLine("there is data to be read...");

                    // read whatever data you need here...

                    byte[] buffer = new byte[client.ReceiveBufferSize];

                    int count = client.GetStream().Read(buffer, 0, buffer.Length);

                    string messsage = Encoding.UTF8.GetString(buffer);

                    Console.WriteLine(messsage);

                    // the data has been successfully read!! :)
                }
            }
        });

        // make sure to start your threads at the end of your code!

        receiveThread.Start();

        // you can stop your threads using "Thread.Abort()" which raises an exception
        // but terminates the thread entirely.

    }
}