namespace AdbMouseFaker
{
    public interface ISendEventWrapper
    {
        public void Send(string deviceSource, int type, int code, int value);
    }
}
