namespace AxisTrix.Results;

public record AxisError
{
    private AxisError(string code, AxisErrorType type)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Error code cannot be null or whitespace.", nameof(code));

        Code = code;
        Type = type;
    }

    public string Code { get; init; }
    public AxisErrorType Type { get; init; }

    public bool IsTransient => Type is AxisErrorType.ServiceUnavailable
        or AxisErrorType.Timeout
        or AxisErrorType.TooManyRequests
        or AxisErrorType.GatewayTimeout;

    public static AxisError InternalServerError(string code) => new(code, AxisErrorType.InternalServerError);
    public static AxisError ValidationRule(string code) => new(code, AxisErrorType.ValidationRule);
    public static AxisError NotFound(string code) => new(code, AxisErrorType.NotFound);
    public static AxisError Conflict(string code) => new(code, AxisErrorType.Conflict);
    public static AxisError BusinessRule(string code) => new(code, AxisErrorType.BusinessRule);
    public static AxisError Unauthorized(string code) => new(code, AxisErrorType.Unauthorized);
    public static AxisError Forbidden(string code) => new(code, AxisErrorType.Forbidden);
    public static AxisError ServiceUnavailable(string code) => new(code, AxisErrorType.ServiceUnavailable);
    public static AxisError Timeout(string code) => new(code, AxisErrorType.Timeout);
    public static AxisError TooManyRequests(string code) => new(code, AxisErrorType.TooManyRequests);
    public static AxisError GatewayTimeout(string code) => new(code, AxisErrorType.GatewayTimeout);
    public static AxisError Mapping(string code) => new(code, AxisErrorType.Mapping);
}