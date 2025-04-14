using DotNetOrderService.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotNetOrderService.Infrastructure.Filters
{
    public class ValidatorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = string.Join("; ", context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                throw new ValidationException($"Validation Error: {errors}", context.ModelState);
            }

            base.OnActionExecuting(context);
        }
    }
}
