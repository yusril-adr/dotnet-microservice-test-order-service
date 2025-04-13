using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DotnetOrderService.Infrastructure.ModelBinder
{
    public partial class SnakeCaseQueryModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext);

            var query = bindingContext.HttpContext.Request.Query;
            var model = Activator.CreateInstance(bindingContext.ModelType);
            var properties = bindingContext.ModelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var snakeCaseName = ToSnakeCase(property.Name);

                if (query.TryGetValue(snakeCaseName, out var value))
                {
                    ProcessPropertyValue(property, value, model, bindingContext);
                }
            }

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }

        private static void ProcessPropertyValue(PropertyInfo property, StringValues value, object model, ModelBindingContext bindingContext)
        {
            var propertyType = property.PropertyType;

            if (IsEnumType(propertyType))
            {
                HandleEnumType(property, value, model, bindingContext);
            }
            else if (IsGuidType(propertyType))
            {
                HandleGuidType(property, value, model, bindingContext);
            }
            else if (IsBooleanType(propertyType))
            {
                HandleBooleanType(property, value, model, bindingContext);
            }
            else
            {
                HandleDefaultType(property, value, model, bindingContext);
            }
        }

        private static bool IsEnumType(Type propertyType)
        {
            return propertyType.IsEnum || Nullable.GetUnderlyingType(propertyType)?.IsEnum == true;
        }

        private static bool IsGuidType(Type propertyType)
        {
            return propertyType == typeof(Guid) || propertyType == typeof(Guid?);
        }

        private static bool IsBooleanType(Type propertyType)
        {
            return propertyType == typeof(bool) || propertyType == typeof(bool?);
        }

        private static void HandleEnumType(PropertyInfo property, StringValues value, object model, ModelBindingContext bindingContext)
        {
            var enumType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            try
            {
                var enumValue = Enum.Parse(enumType, value.First(), true);
                property.SetValue(model, enumValue);
            }
            catch (ArgumentException)
            {
                bindingContext.ModelState.AddModelError(property.Name, $"Invalid value for enum {enumType.Name}");
            }
        }

        private static void HandleGuidType(PropertyInfo property, StringValues value, object model, ModelBindingContext bindingContext)
        {
            if (Guid.TryParse(value.First(), out var guidValue))
            {
                property.SetValue(model, guidValue);
            }
            else
            {
                bindingContext.ModelState.AddModelError(property.Name, "Invalid Guid format");
            }
        }

        private static void HandleBooleanType(PropertyInfo property, StringValues value, object model, ModelBindingContext bindingContext)
        {
            var firstValue = value.First().Trim();

            if (bool.TryParse(firstValue, out var boolValue) ||
                firstValue == "1" ||
                firstValue.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                property.SetValue(model, true);
            }
            else if (firstValue == "0" ||
                     firstValue.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                property.SetValue(model, false);
            }
            else
            {
                bindingContext.ModelState.AddModelError(property.Name, "Invalid boolean format");
            }
        }

        private static void HandleDefaultType(PropertyInfo property, StringValues value, object model, ModelBindingContext bindingContext)
        {
            try
            {
                var convertedValue = Convert.ChangeType(value.First(), property.PropertyType);
                property.SetValue(model, convertedValue);
            }
            catch (Exception)
            {
                bindingContext.ModelState.AddModelError(property.Name, $"Invalid format for {property.PropertyType.Name}");
            }
        }

        private static string ToSnakeCase(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            // Convert to snake_case (from PascalCase or camelCase)
            var regex = MyRegex();
            return regex.Replace(str, "$1_$2").ToLower();
        }

        [GeneratedRegex("([a-z])([A-Z])")]
        private static partial Regex MyRegex();
    }

    public class SnakeCaseQueryModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            ArgumentNullException.ThrowIfNull(nameof(context));

            // Apply the SnakeCaseQueryModelBinder to all query parameters
            if (context.BindingInfo.BindingSource == BindingSource.Query)
            {
                return new SnakeCaseQueryModelBinder();
            }

            return null;
        }
    }
}
