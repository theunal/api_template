using System.Net;

namespace Core.Utilities.Results
{
    public class ErrorResult : Result
    {
        public ErrorResult(string message, HttpStatusCode? statusCode = null) : base(false, message, statusCode) { }

        public ErrorResult(HttpStatusCode? statusCode = null) : base(false, statusCode) { }
    }
}
