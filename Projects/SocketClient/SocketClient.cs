using System.Net.Sockets;
using System.Text;


namespace SocketClient;

public class SocketClient : ISocketClient
{
    public async Task StartClient(CancellationToken cancellationToken)
    {
        try
        {
            // Set up the client socket
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            await ConnectToServerAsync(clientSocket, cancellationToken);

            Console.WriteLine("Connected to the server. Type 'exit' to disconnect.");

            // Start a new task for receiving data from the server
            var receiveTask = ReceiveDataAsync(clientSocket, cancellationToken);

            while (true)
            {
                string input = Console.ReadLine();

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    // Gracefully disconnect from the server
                    await DisconnectFromServerAsync(clientSocket, cancellationToken);
                    break;
                }

                // Send data to the server
                await SendDataAsync(clientSocket, input, cancellationToken);
            }

            // Wait for the receive task to complete
            await receiveTask;

            // Close the client socket
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
        catch (SocketException ex)
        {
            Console.WriteLine("A socket error occurred: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }

        Console.WriteLine("Disconnected from the server.");
    }

    private async Task ConnectToServerAsync(Socket socket, CancellationToken cancellationToken)
    {
        await socket.ConnectAsync("127.0.0.1", 8080, cancellationToken);
    }

    private async Task DisconnectFromServerAsync(Socket socket, CancellationToken cancellationToken)
    {
        await socket.DisconnectAsync(reuseSocket: true, cancellationToken);
    }

    private async Task SendDataAsync(Socket socket, string data, CancellationToken cancellationToken)
    {
        byte[] sendData = Encoding.ASCII.GetBytes(data);
        await socket.SendAsync(sendData, SocketFlags.None, cancellationToken);
    }

    private async Task ReceiveDataAsync(Socket socket, CancellationToken cancellationToken)
    {
        byte[] buffer = new byte[1024];

        try
        {
            while (true)
            {
                int bytesRead = await socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken);
                if (bytesRead == 0)
                {
                    // Client has disconnected gracefully
                    break;
                }
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Data received from server: " + dataReceived);
            }
        }
        catch (SocketException ex)
        {
            if (ex.SocketErrorCode == SocketError.ConnectionReset)
            {
                // Server connection was abruptly terminated
                Console.WriteLine("Server connection was abruptly terminated.");
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
    }
}
