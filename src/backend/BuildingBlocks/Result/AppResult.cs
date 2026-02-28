namespace Result;

public class AppResult
{
    private readonly List<AppError> _errors = new();
    
    private bool _isSuccess;

    public bool IsSuccess => _isSuccess;
    
    public bool IsFailure => !IsSuccess;

    public List<AppError> Errors => _errors;

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
    
    public static AppResult Failure(AppError error) => new(false, error);
    
    public static AppResult Failure(AppError[] errors) => new(false, errors);

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

public class AppResult<TResult> : AppResult
{
    public TResult Result { get; }
    
    public AppResult(bool isSuccess, TResult result, AppError error) : this(isSuccess, result, [error])
    {
        
    }

    public AppResult(bool isSuccess, TResult result, AppError[] errors) : base(isSuccess, errors)
    {
        Result = result;
    }
    
    public static AppResult<TResult> Success(TResult result) => new(true, result, []);
}