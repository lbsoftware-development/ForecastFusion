using Azure.Identity;
using System.Net;

namespace ForecastFusion.Application
{
    public class Result<T>
    {
        public T Value { get; }
        public Exception Error { get; }
        public bool IsSuccess => Error == null;

        public HttpStatusCode? HttpStatusCode { get; set; }

        private Result(T value, Exception error)
        {
            Value = value;
            Error = error;
        }

        public Result(T value, Exception error, HttpStatusCode httpStatusCode)
        {
            Value = value;
            Error = error;
            HttpStatusCode = httpStatusCode;
        }

        public static Result<T> Success(T value) => new Result<T>(value, null);
        public static Result<T> Failure(Exception error) => new Result<T>(default(T), error);

        public static Result<T> Failure(T value, Exception error, HttpStatusCode httpStatusCode) => new Result<T>(value, error, httpStatusCode);

    }

    public class Result
    {
        public Exception Error { get; }
        public bool IsSuccess => Error == null;

        public HttpStatusCode? HttpStatusCode { get; set; }

        private Result(Exception error)
        {
            Error = error;
        }
              

        public static Result Success() => new Result(null);
        public static Result Failure(Exception error) => new Result(error);        
    }
}
