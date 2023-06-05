namespace SocketServer;

class Program
{
    private static readonly CancellationTokenSource CanToken = new ();

    static Task Main(string[] args)
    {
        var socketServer = new SocketServer();

        return socketServer.StartServer(CanToken.Token);
    }
}