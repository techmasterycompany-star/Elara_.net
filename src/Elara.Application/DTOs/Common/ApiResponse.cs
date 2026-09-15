namespace Elara.Application.DTOs.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> SuccessResponse(T data)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data
            };
        }

        public static ApiResponse<T> FailResponse(T data)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = data
            };
        }
    }
}
