using System.Net;

namespace Core.Utilities.Results
{
    public class Result : IResult
    {
        public bool Success { get; }
        public string Message { get; }
        public HttpStatusCode? StatusCode { get; }

        public Result(bool Success, HttpStatusCode? statusCode = null)
        {
            this.Success = Success;
            StatusCode = statusCode;
        }

        public Result(bool Success, string Message, HttpStatusCode? statusCode = null) : this(Success)
        {
            this.Message = Message;
            StatusCode = statusCode;
        }
    }
}
