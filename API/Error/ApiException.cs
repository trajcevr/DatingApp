using System;

namespace API.Error
{
    public class ApiException
    {
        private string v;

        public ApiException(int statusCode, string v)
        {
            StatusCode = statusCode;
            this.v = v;
        }


        public ApiException(int statusCode, string message, string details = null)
        {
            StatusCode = statusCode;
            Message = message ?? "An unexpected error occurred."; // Default message if null
            Details = details;
        }



        // HTTP Status Code (e.g., 404, 500, etc.)
        public int StatusCode { get; set; }

        // General message for the error
        public string Message { get; set; }

        // Additional details for debugging
        public string Details { get; set; }
    }
}
