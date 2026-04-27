namespace Result;

public record AppResult
{
    private readonly List<AppError> _errors = new();
    
    private bool _isSuccess;

    public bool IsSuccess => _isSuccess;
    
    public bool IsFailure => !IsSuccess;

    public IEnumerable<AppError> Errors => _errors;

    public AppResult(bool isSuccess, AppError error) : this(isSuccess, [error])
    {
        
    }

    public AppResult(bool isSuccess, AppError[] errors)
    {
        if (isSuccess && errors.Length != 0 || !isSuccess && errors.Length == 0)
        {
            throw new ArgumentNullException(nameof(errors));
        }
        
        _isSuccess = isSuccess;

        if (errors.Length != 0)
        {
            _errors.AddRange(errors);
        }
    }
    
    public static AppResult Success() => new(true, []);
    
    public static AppResult Failure(AppErrorType type, string message, string? code = null) => 
        new(false, new AppError(type, message, Code: code));
    
    public static AppResult Failure(AppError error) => new(false, error);
    
    public static AppResult Failure(AppError[] errors) => new(false, errors);
    
    public static AppResult Failure(AppResult result) => new(false, result.Errors.ToArray());

    public void AddError(AppError error)
    {
        _isSuccess = false;
        
        _errors.Add(error);
    }
    
    public void AddErrors(AppError[] errors)
    {
        _isSuccess = false;
        
        _errors.AddRange(errors);
    }
}

public record AppResult<TResult> : AppResult
{
    public TResult? Result { get; }
    
    public AppResult(bool isSuccess, TResult? result, AppError error) : this(isSuccess, result, [error])
    {
        
    }

    public AppResult(bool isSuccess, TResult? result, AppError[] errors) : base(isSuccess, errors)
    {
        Result = result;
    }
    
    public static AppResult<TResult> Success(TResult result) => new(true, result, []);

    public new static AppResult<TResult> Failure(AppErrorType type, string message, string? code = null) =>
        new(false, default, new AppError(type, message, Code: code));
    
    public new static AppResult<TResult> Failure(AppError error) => new(false, default, error);
    
    public new static AppResult<TResult> Failure(AppError[] errors) => new(false, default, errors);

    public new static AppResult<TResult> Failure(AppResult result) => new(false, default, result.Errors.ToArray());
}