using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ICMarket.API.Filters;

/// <summary>
/// Global exception filter that catches <see cref="ValidationException"/> thrown by the
/// FluentValidation pipeline and converts them into structured 400 Bad Request responses.
/// </summary>
public class ValidationExceptionFilter : IExceptionFilter
{
	public void OnException(ExceptionContext context)
	{
		if (context.Exception is ValidationException ex)
		{
			var errors = ex.Errors.Select(e => e.ErrorMessage);
			context.Result = new BadRequestObjectResult(new { errors });
			context.ExceptionHandled = true;
		}
	}
}
