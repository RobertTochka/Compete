namespace Compete_POCO_Models.Infrastrcuture.Data
{
    public class ApplicationError : Exception
    {
        public ApplicationError(string? message) : base(message)
        {
        }
    }
    public class LobbySmoothlyError : ApplicationError
    {
        public LobbySmoothlyError(string? message) : base(message)
        {
        }
    }
}
