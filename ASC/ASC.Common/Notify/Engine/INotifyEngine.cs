namespace ASC.Notify.Engine
{
    internal interface INotifyEngine
    {
        NotifyResult Request(NotifyRequest request);
        void QueueRequest(NotifyRequest request);
        event TransferRequest AfterTransferRequest;
        event TransferRequest BeforeTransferRequest;
    }
}