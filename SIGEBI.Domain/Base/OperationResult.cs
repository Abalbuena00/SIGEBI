namespace SIGEBI.Domain.Base;

public sealed class OperationResult
{
    public bool IsSuccess { get; }

    public string? Error { get; }

    private OperationResult(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static OperationResult Success()
    {
        return new OperationResult(true, null);
    }

    public static OperationResult Failure(string error)
    {
        return new OperationResult(false, error);
    }
}