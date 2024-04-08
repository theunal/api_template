using System.Net;

namespace Core.Utilities.Results
{
    public class SuccessResult : Result
    {
        public SuccessResult(string message, HttpStatusCode? statusCode = null) : base(true, message, statusCode) { }
        public SuccessResult(HttpStatusCode? statusCode = null) : base(true, statusCode) { }
    }
}
