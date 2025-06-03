namespace InfoSurge.Core
{
    public class NoEntityException : Exception
    {
        public NoEntityException(string message)
            : base(message)
        {
        }
    }
}
