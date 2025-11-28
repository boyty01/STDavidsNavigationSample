using System.Net;

namespace StDavidsQRNavigation.Services
{


    public class ServiceResult<T>
    {
        public bool IsSuccess { get; private set; }

        public HttpStatusCode HttpCode;
        public T? Data { get; private set; }
        public string? ErrorMessage { get; private set; }
        public object? ErrorDetails { get; private set; }


        // Success result
        public static ServiceResult<T> Success(HttpStatusCode httpCode, T data)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                HttpCode = httpCode,
                Data = data
            };
        }

        // Failure result
        public static ServiceResult<T> Failure(HttpStatusCode httpCode, string? errorMessage, object? errorDetails = null)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                HttpCode = httpCode,
                ErrorMessage = errorMessage,
                ErrorDetails = errorDetails
            };
        }
    }
}
