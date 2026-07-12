namespace SIGEBI.Application.Common;

public sealed class ApplicationResult<T>
{
    public bool IsSuccess { get; }

    public T? Data { get; }

    public string? Error { get; }

    private ApplicationResult(bool isSuccess, T? data, string? error)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
    }

    public static ApplicationResult<T> Success(T data)
    {
        return new ApplicationResult<T>(true, data, null);
    }

    public static ApplicationResult<T> Failure(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("El mensaje de error es obligatorio.");

        return new ApplicationResult<T>(false, default, error.Trim());
    }
}