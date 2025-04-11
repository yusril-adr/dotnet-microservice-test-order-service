using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DotNetService.Infrastructure.Dtos
{
    public class Validation
    {
        public string Key;
        public string Value;

        public IDictionary<string, string> ConvertToDictionary()
        {
            var validationParsed = new Dictionary<string, string>
            {
                { "key", this.Key },
                { "value", this.Value }
            };

            return validationParsed;
        }
    }

    public class ErrorValidation
    {
        public static List<IDictionary<string, string>> ErrorModel(ModelStateDictionary modelState)
        {
            if (modelState == null) return null;

            List<KeyValuePair<string, ModelStateEntry>> errors = modelState
                .Where(modelError => modelError.Value.Errors.Count > 0)
                .ToList();

            List<IDictionary<string, string>> errorParsed = new List<IDictionary<string, string>>();
            foreach (KeyValuePair<string, ModelStateEntry> error in errors)
            {
                Validation errorData = new()
                {
                    Key = error.Key,
                    Value = error.Value.Errors.First().ErrorMessage
                };

                errorParsed.Add(errorData.ConvertToDictionary());
            }

            return errorParsed;
        }
    }
}