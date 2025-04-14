using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DotNetOrderService.Infrastructure.Exceptions
{
    public class ValidationException(string message, ModelStateDictionary modelState) : Exception(message)
    {
        public ModelStateDictionary ModelState { get; set; } = modelState;
    }
}