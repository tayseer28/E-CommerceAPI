using E_Commerce.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    public class ApiBaseController : ControllerBase
    {
        public static ActionResult<T> ToActionResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(result.data);
            else
                return ToProblem(result.Errors);
        }


        public static ActionResult ToActionResult(Result result)
        {
            if (result.IsSuccess)
                return new OkResult();
            else
                return ToProblem(result.Errors);
        }
        protected static ObjectResult ToProblem(IReadOnlyList<Error> errors)
        {
            var firstError = errors[0];
            var statusCode = firstError.ErrorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = firstError.Code,
                Detail = string.Join(Environment.NewLine, errors.Select(e => e.Description)),
                Extensions = { ["errors"] = errors },
                Type = $"https://httpstatuses.com/{statusCode}",
            };
            return new ObjectResult(problemDetails)
            {
                StatusCode = statusCode
            };
        }
    }
}
