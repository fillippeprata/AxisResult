namespace AxisTrix.Results;

public enum AxisErrorType
{
    ServiceUnavailable = 1,
    Timeout = 2,
    TooManyRequests = 3,
    GatewayTimeout = 4,
    ValidationRule = 5,
    NotFound = 6,
    Conflict = 7,
    BusinessRule = 8,
    Unauthorized = 9,
    Forbidden = 10,
    Mapping = 11,
    InternalServerError = 12
}
