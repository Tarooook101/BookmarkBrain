namespace BookMarkBrain.Core.ServiceInterfaces;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public List<string> Errors { get; set; } = new List<string>();

    public static ServiceResult<T> SuccessResult(T data, string message = null)
    {
        return new ServiceResult<T>
        {
            Success = true,
            Message = message ?? "Operation completed successfully",
            Data = data
        };
    }

    public static ServiceResult<T> FailureResult(string message, List<string> errors = null)
    {
        return new ServiceResult<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}