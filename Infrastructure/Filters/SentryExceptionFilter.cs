using DotNetOrderService.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Sentry.Extensibility;

namespace DotNetOrderService.Infrastructure.Filters
{
    public class SentryExceptionFilter : IExceptionFilter
    {
        private static List<Type> FilteredExceptionTypes()
        {
            // Write your custom exception that should be filtered here
            return [
                typeof(FluentValidation.ValidationException),
                typeof(BadHttpRequestException),
                typeof(BadRequest),
                typeof(DataNotFoundException),
                typeof(ValidationException),
                typeof(NotAllowedException),
                typeof(UnauthenticatedException),
                typeof(UnprocessableEntityException),
                typeof(BusinessException)
            ];
        }

        public bool Filter(Exception ex)
        {
            if (FilteredExceptionTypes().Contains(ex.GetType())) return true;

            return false;
        }
    }
}
