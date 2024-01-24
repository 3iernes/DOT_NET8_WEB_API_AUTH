namespace DOT_NET_WEB_API_AUTH.Core.App
{
    public class SimpleAppResponse<T>
    {
        public bool IsSuccess { get; private set; } = true;
        public string? Message { get; private set; }
        public T? Data { get; private set; }
        public Dictionary<string, string[]>? ErrorDetail { get; private set; }

        public SimpleAppResponse<T> Success(T? data = default, string? message = null)
        {
            IsSuccess = true;
            Data = data;
            Message = message;
            return this;
        }

        public SimpleAppResponse<T> Error(string message, Dictionary<string, string[]>? errorDetails = null)
        {
            IsSuccess = false;
            Message = message;
            ErrorDetail = errorDetails;
            return this;
        }
    }
}
