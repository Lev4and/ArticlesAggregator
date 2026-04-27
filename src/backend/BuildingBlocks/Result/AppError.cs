namespace Result;

public record AppError(AppErrorType Type, string Message, string? Code = null);