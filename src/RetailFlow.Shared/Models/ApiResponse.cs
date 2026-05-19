namespace RetailFlow.Shared.Models
{
    /// <summary>
    /// Standard API response wrapper used across all endpoints.
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Success")
        {
            return new ApiResponse<T> { Success = true, Message = message, Data = data };
        }

        public static ApiResponse<T> Fail(string message)
        {
            return new ApiResponse<T> { Success = false, Message = message };
        }
    }

    public class ApiErrorResponse
    {
        public bool Success { get; set; } = false;
        public string ErrorCode { get; set; }
        public string Message { get; set; }

        public static ApiErrorResponse Create(string errorCode, string message)
        {
            return new ApiErrorResponse { ErrorCode = errorCode, Message = message };
        }
    }
}
