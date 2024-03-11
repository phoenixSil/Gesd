using System.Net;

namespace Gesd.Entite.Responses
{
    public class RequestResponse
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }

        public RequestResponse(int statusCode, bool success, string message, List<string> errors)
        {
            StatusCode = statusCode;
            Success = success;
            Message = message;
            Errors = errors;
        }

        public RequestResponse()
        {

        }

    }
}
