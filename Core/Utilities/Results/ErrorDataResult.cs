using System.Net;

namespace Core.Utilities.Results
{
    public class ErrorDataResult<T> : DataResult<T>
    {
        // sadece data
        public ErrorDataResult(T data, HttpStatusCode? statusCode = null) : base(data, false, statusCode) { }

        // data ve message
        public ErrorDataResult(T data, string message, HttpStatusCode? statusCode = null) : base(data, false, message, statusCode) { }

        // default data ve message
        public ErrorDataResult(string message, HttpStatusCode? statusCode = null) : base(default!, false, message, statusCode) { }

        // sadece default data
        public ErrorDataResult(HttpStatusCode? statusCode = null) : base(default!, false, statusCode) { }
    }
}
