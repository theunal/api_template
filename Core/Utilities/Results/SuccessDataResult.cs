using System.Net;

namespace Core.Utilities.Results
{
    public class SuccessDataResult<T> : DataResult<T>
    {
        // sadece data
        public SuccessDataResult(T data, HttpStatusCode? statusCode = null) : base(data, true, statusCode) { }

        // data ve message
        public SuccessDataResult(T data, string message, HttpStatusCode? statusCode = null) : base(data, true, message, statusCode) { }

        // default data ve message
        public SuccessDataResult(string message, HttpStatusCode? statusCode = null) : base(default!, true, message, statusCode) { }

        // sadece default data
        public SuccessDataResult(HttpStatusCode? statusCode = null) : base(default!, true, statusCode) { }
    }
}
