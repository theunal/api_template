using System.Net;

namespace Core.Utilities.Results
{
    public interface IResult // ana result 
    {
        public bool Success { get; }
        public string Message { get; }
        public HttpStatusCode? StatusCode { get; }
    }
}
