using System.Text.Json.Serialization;

namespace E_Commerce.Application.Common
{
    public sealed record Error(string Code, string Description, ErrorType ErrorType = ErrorType.Failure)
    {
        public static Error Failure(string code = "General.Failure", string description = "A failure has occurred.") =>
            new Error(code, description, ErrorType.Failure);
        public static Error Validation(string code = "General.Validation", string description = "A validation error has occurred.") =>
            new Error(code, description, ErrorType.Validation);
        public static Error NotFound(string code = "General.NotFound", string description = "The requested resource was not found.") =>
            new Error(code, description, ErrorType.NotFound);
        public static Error Conflict(string code = "General.Conflict", string description = "A conflict has occurred.") =>
            new Error(code, description, ErrorType.Conflict);
        public static Error Unauthorized(string code = "General.Unauthorized", string description = "The request is not authorized.") =>
            new Error(code, description, ErrorType.Unauthorized);
        public static Error Forbidden(string code = "General.Forbidden", string description = "The request is forbidden.") =>
            new Error(code, description, ErrorType.Forbidden);
        public static Error InvalidCredentials(string code = "General.InvalidCredentials", string description = "The provided credentials are invalid.") =>
            new Error(code, description, ErrorType.InvalidCredentials);
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        Unauthorized = 4,
        Forbidden = 5,
        InvalidCredentials = 6
    }
}