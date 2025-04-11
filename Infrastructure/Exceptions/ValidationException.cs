using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DotNetService.Infrastructure.Exceptions
{
    public class ValidationException(string message, ModelStateDictionary modelState) : Exception(message)
    {
        public ModelStateDictionary ModelState { get; set; } = modelState;
    }
}