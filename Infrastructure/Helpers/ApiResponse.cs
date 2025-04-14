using System.Net;
using System.Runtime.Serialization;
using DotNetOrderService.Infrastructure.Dtos;

namespace DotNetOrderService.Infrastructure.Helpers
{
    public class ErrorUtility
    {
        public static List<IDictionary<string, string>> CreateSingleErrorValidation(string key, string field)
        {
            IDictionary<string, string> errorsValidation = ErrorUtility.SetErrorValidation(key, field);
            List<IDictionary<string, string>> validations = [errorsValidation];

            return validations;
        }
        public static IDictionary<string, string> SetErrorValidation(string key, string field)
        {
            IDictionary<string, string> validation = new Dictionary<string, string>
            {
                { "key", key },
                { "field", field }
            };
            return validation;
        }
    }

    [DataContract]
    public abstract class ApiResponse
    {
        [DataMember]
        public string Version { get { return "1.0.0"; } }
    }

    public class ApiResponseData<T>(HttpStatusCode statusCode, T data) : ApiResponse
    {
        [DataMember]
        public int StatusCode { get; set; } = (int)statusCode;

        [DataMember(EmitDefaultValue = true)]
        public T Data { get; set; } = data;
    }

    public class ResponseDataList<T>(List<T> items, int count)
    {
        [DataMember(EmitDefaultValue = true)]
        public List<T> Items { get; set; } = items;

        [DataMember(EmitDefaultValue = true)]
        public int Count { get; set; } = count;
    }

    public class ApiResponseDataList<T>(HttpStatusCode statusCode, List<T> items, int count) : ApiResponse
    {
        [DataMember]
        public int StatusCode { get; set; } = (int)statusCode;

        [DataMember(EmitDefaultValue = true)]
        public ResponseDataList<T> Items { get; set; } = new ResponseDataList<T>(items, count);
    }

    public class ApiResponsePagination<T>(HttpStatusCode statusCode, PaginationModel<T> paginationModel) : ApiResponse
    {
        [DataMember]
        public int StatusCode { get; set; } = (int)statusCode;


        [DataMember(EmitDefaultValue = true)]
        public ResponsePagination<T> Data { get; set; } = new ResponsePagination<T>(paginationModel);
    }

    public class ResponsePagination<T>(PaginationModel<T> paginationModel)
    {
        [DataMember(EmitDefaultValue = true)]
        public List<T> Items { get; set; } = paginationModel.Data;

        [DataMember(EmitDefaultValue = true)]
        public PaginationMeta Meta { get; set; } = paginationModel.Meta;
    }

    public class ApiResponseError(HttpStatusCode statusCode, string errorMessage, object errors = null, string stackTrace = null) : ApiResponse
    {
        [DataMember(EmitDefaultValue = true)]
        public string ErrorMessage { get; set; } = errorMessage;

        [DataMember(EmitDefaultValue = true)]
        public string StackTrace { get; set; } = stackTrace;

        [DataMember(EmitDefaultValue = true)]
        public object Errors { get; set; } = errors;

        [DataMember]
        public int StatusCode { get; set; } = (int)statusCode;
    }
}
