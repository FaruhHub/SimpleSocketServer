using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketServer;

public class SocketServer : ISocketServer
{
    private static readonly List<Socket> _clients = new ();
    private static readonly object _lockObject = new ();

    public async Task StartServer(CancellationToken cancellationToken)
    {
        try
        {
            // Set up the server socket
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 8080);
            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(10);

            Console.WriteLine("Server started. Waiting for a client to connect...");

            while (true)
            {
                // Accept a client connection asynchronously
                var acceptTask = serverSocket.AcceptAsync(cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                await HandleClientAsync(acceptTask, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    private async Task HandleClientAsync(ValueTask<Socket> acceptTask, CancellationToken cancellationToken)
    {
        Socket clientSocket = await acceptTask;

        try
        {
            Console.WriteLine("Client connected. Thread ID: " + Environment.CurrentManagedThreadId);

            // Add the client socket to the list
            lock (_lockObject)
            {
                _clients.Add(clientSocket);
            }

            // Start receiving data from the client asynchronously
            byte[] buffer = new byte[1024];
            while (true)
            {
                int bytesRead = await clientSocket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken);
                if (bytesRead == 0)
                {
                    // Client has disconnected gracefully
                    break;
                }

                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Data received from client: " + dataReceived);

                // Process the received data asynchronously
                string processedData = await ProcessDataAsync(dataReceived, cancellationToken);

                // Send the processed data back to the client asynchronously
                byte[] sendData = Encoding.ASCII.GetBytes(processedData);
                await clientSocket.SendAsync(sendData, SocketFlags.None, cancellationToken);
            }
        }
        catch (SocketException ex)
        {
            if (ex.SocketErrorCode == SocketError.ConnectionReset ||
                ex.SocketErrorCode == SocketError.ConnectionAborted)
            {
                // Client connection was abruptly terminated
                Console.WriteLine("Client connection was abruptly terminated.");
            }
            else
            {
                Console.WriteLine("A socket error occurred: " + ex.Message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
        finally
        {
            // Close the client socket and remove it from the list
            clientSocket.Close();
            lock (_lockObject)
            {
                _clients.Remove(clientSocket);
            }

            Console.WriteLine("Client disconnected. Thread ID: " + Environment.CurrentManagedThreadId);
        }
    }

    private async Task<string> ProcessDataAsync(string data, CancellationToken cancellationToken)
    {
        // Perform some processing on the data received asynchronously
        await Task.Delay(1000, cancellationToken); // Simulating some processing time
        // ...
        // Return the processed data
        return "Processed as " + data;
    }
}
