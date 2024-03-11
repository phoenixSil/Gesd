using System.Net;

namespace Gesd.Entite.Responses
{
    public class ApiResponse<T> : RequestResponse 
    {
        public T Data { get; set; }

        public ApiResponse(int statusCode, bool success, string message, List<string> errors, T data)
            : base(statusCode, success, message, errors)
        {
            Data = data;
        }

        public ApiResponse()
        {
        }

        public static ApiResponse<T> CreateNotFoundResponse(string message)
        {
            return new ApiResponse<T>
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Data = default,
                Success = true,
                Message = message
            };
        }

        public static ApiResponse<T> CreateSuccessResponse(T data, string message = "Opération terminée avec succès !!")
        {
            return new ApiResponse<T>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data,
                Success = true,
                Message = message
            };
        }

        public static ApiResponse<T> CreateErrorResponse(Exception ex, string message)
        {
            return new ApiResponse<T>
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Data = default,
                Success = false,
                Message = message,
                Errors = new List<string> { ex.Message }
            };
        }

        public static ApiResponse<T> CreateErrorResponse(HttpStatusCode statusCode, string message, List<string>? listError)
        {
            return new ApiResponse<T>
            {
                StatusCode = (int)statusCode,
                Success = false,
                Message = message,
                Errors = listError
            };
        }

        public static ApiResponse<T> CreateBadRequestResponse(string message)
        {
            return new ApiResponse<T>
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Success = false,
                Message = message,
            };
        }
    }
}
