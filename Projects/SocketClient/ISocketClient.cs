namespace SocketClient;

public interface ISocketClient
{
    Task StartClient(CancellationToken cancellationToken);
}