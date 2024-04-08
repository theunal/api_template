using System.Net;

namespace Core.Utilities.Results
{
    public class DataResult<T> : Result, IDataResult<T>
    {
        public T Data { get; }
        public DataResult(T data, bool success, HttpStatusCode? statusCode = null) : base(success, statusCode)
        {
            Data = data;
        }

        public DataResult(T data, bool success, string message, HttpStatusCode? statusCode = null) : base(success, message, statusCode)
        {
            Data = data;
        }
    }
}
