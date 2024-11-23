namespace Bookstore.Services.Exceptions
{
    public class DbConcorrencyException : ApplicationException
    {
        public DbConcorrencyException(string? message) : base(message)
        {
        }
    }
}
