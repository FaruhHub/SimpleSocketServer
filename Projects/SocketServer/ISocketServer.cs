namespace SocketServer;

public interface ISocketServer
{
    Task StartServer(CancellationToken cancellationToken);
}