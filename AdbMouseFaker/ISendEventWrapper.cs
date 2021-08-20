namespace AdbMouseFaker
{
    public interface ISendEventWrapper
    {
        public void Send(int type, int code, int value);
    }
}
