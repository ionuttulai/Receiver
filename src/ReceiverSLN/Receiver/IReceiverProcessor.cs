namespace Receiver
{
    public interface IReceiverProcessor
    {
        void HandlePackage(object package, long sequence);
    }
}