namespace SocketClient;

class Program
{
    private static readonly CancellationTokenSource CanToken = new ();

    static Task Main(string[] args)
    {
        var socketClient = new SocketClient();
        return socketClient.StartClient(CanToken.Token);
    }
}