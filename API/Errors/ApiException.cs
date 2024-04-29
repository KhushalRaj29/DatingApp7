namespace API.Errors
{
    public class ApiException
    {
        public ApiException(int statusCode, string message, string details)
        {
            statusCode = StatusCode;
            message = Message;
            details = Details;
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}