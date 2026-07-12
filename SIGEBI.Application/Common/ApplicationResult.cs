namespace SIGEBI.Application.Common;

public sealed class ApplicationResult
{
    public bool IsSuccess { get; }

    public string? Error { get; }

    private ApplicationResult(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static ApplicationResult Success()
    {
        return new ApplicationResult(true, null);
    }

    public static ApplicationResult Failure(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("El mensaje de error es obligatorio.");

        return new ApplicationResult(false, error.Trim());
    }
}