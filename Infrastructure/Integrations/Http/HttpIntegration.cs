

using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;
using DotnetOrderService.Constants.Logger;
using DotnetOrderService.Infrastructure.Shareds;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace DotnetOrderService.Infrastructure.Integrations.Http
{
    public class HttpIntegration(
        ILoggerFactory loggerFactory,
        IConfiguration config,
        HttpClient httpClient
        )
    {

        public readonly IConfiguration _config = config;

        public readonly HttpClient _httpClient = httpClient;

        public readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.INTEGRATION);

        public Dictionary<string, string> Params { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public async Task<ResBody> Get<ResBody>(string url, bool? cleanUp = true)
        {
            _logger.LogInformation("HttpIntegration Request to url : " + url);

            try
            {
                var response = await _httpClient.GetAsync(PrepareUrl(url));
                response.EnsureSuccessStatusCode();

                CleanUp();

                var data = Utils.JsonDeserializeResponse<ResBody>(response.Content);
                _logger.LogInformation("HttpIntegration Response from GET : " + JsonSerializer.Serialize(data));

                return data;
            }
            catch (HttpRequestException err)
            {
                _logger.LogInformation("HttpIntegration Error POST : " + err.Message + ", Status : " + err.StatusCode + ", : Data : " + JsonConvert.SerializeObject(err.Data).ToString());

                throw;
            }
        }

        public async Task<ResBody> PostJson<ReqBody, ResBody>(string url, ReqBody body, bool? cleanUp = true)
        {
            _logger.LogInformation("HttpIntegration Request to url : " + url);

            try
            {
                var jsonData = JsonSerializer.Serialize(body);
                var reqBody = new StringContent(jsonData, Encoding.UTF8, "application/json");

                _logger.LogInformation("HttpIntegration Response from POST : " + jsonData);

                var response = await _httpClient.PostAsync(PrepareUrl(url), reqBody);
                response.EnsureSuccessStatusCode();
                CleanUp();

                var data = Utils.JsonDeserializeResponse<ResBody>(response.Content);
                _logger.LogInformation("HttpIntegration Response from POST : " + JsonSerializer.Serialize(data));

                return data;
            }
            catch (HttpRequestException err)
            {
                _logger.LogInformation("HttpIntegration Error POST : " + err.Message + ", Status : " + err.StatusCode + ", : Data : " + JsonConvert.SerializeObject(err.Data).ToString());

                throw;
            }
            catch (Exception err)
            {
                _logger.LogInformation("HttpIntegration Error POST : " + err.Message + ", : Data : " + JsonConvert.SerializeObject(err.Data).ToString());

                throw;
            }
        }

        public async Task<bool> PutFormUrlEncodedWithoutResponse(string url, IDictionary<string, string> body)
        {
            _logger.LogInformation("HttpIntegration Request to url : " + url);

            try
            {
                var formBody = new FormUrlEncodedContent(body);

                _logger.LogInformation("HttpIntegration Request from PUT : " + formBody.ToString());

                var response = await _httpClient.PutAsync(PrepareUrl(url), formBody);
                response.EnsureSuccessStatusCode();
                CleanUp();

                _logger.LogInformation("HttpIntegration Response from PUT : " + JsonSerializer.Serialize(response.Content));

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException err)
            {
                _logger.LogInformation("HttpIntegration Error PUT : " + err.Message + ", Status : " + err.StatusCode + ", : Data : " + JsonConvert.SerializeObject(err.Data).ToString());

                throw;
            }
            catch (Exception err)
            {
                _logger.LogInformation("HttpIntegration Error PUT : " + err.Message + ", : Data : " + JsonConvert.SerializeObject(err.Data).ToString());

                throw;
            }
        }

        public async Task<ResBody> PostFormUrlEncoded<ResBody>(string url, IDictionary<string, string> body)
        {
            _logger.LogInformation("HttpIntegration Request to url : " + url);

            try
            {
                var formBody = new FormUrlEncodedContent(body);

                _logger.LogInformation("HttpIntegration Request from POST : " + JsonConvert.SerializeObject(body));

                var response = await _httpClient.PostAsync(PrepareUrl(url), formBody);
                response.EnsureSuccessStatusCode();
                CleanUp();

                var data = Utils.JsonDeserializeResponse<ResBody>(response.Content);

                _logger.LogInformation("HttpIntegration Response from POST : " + JsonSerializer.Serialize(data));

                return data;
            }
            catch (HttpRequestException err)
            {

                _logger.LogInformation("HttpIntegration Error POST : " + err.Message + ", Status : " + err.StatusCode + ", : Data : " + JsonConvert.SerializeObject(err.Data).ToString());

                throw;
            }
            catch (Exception err)
            {
                _logger.LogInformation("HttpIntegration Error POST : " + err.Message + ", : Data : " + JsonConvert.SerializeObject(err.Data).ToString());

                throw;
            }
        }

        public async Task<ResBody> PostFormData<ResBody>(string url, IDictionary<string, string> body)
        {
            _logger.LogInformation("HttpIntegration Request to url : " + url);

            try
            {
                var formBody = new MultipartFormDataContent();
                foreach (KeyValuePair<string, string> entry in body)
                {
                    formBody.Add(new StringContent(entry.Value), entry.Key);
                }

                _logger.LogInformation("HttpIntegration Request from POST : " + formBody.ToString());

                var response = await _httpClient.PostAsync(PrepareUrl(url), formBody);
                response.EnsureSuccessStatusCode();
                CleanUp();

                var data = Utils.JsonDeserializeResponse<ResBody>(response.Content);

                _logger.LogInformation("HttpIntegration Response from POST : " + JsonSerializer.Serialize(data));

                return data;
            }
            catch (HttpRequestException err)
            {

                _logger.LogInformation("HttpIntegration Error POST : " + err.Message + ", Status : " + err.StatusCode + ", : Data : " + JsonConvert.SerializeObject(err.Data).ToString());

                throw;
            }
            catch (Exception err)
            {
                _logger.LogInformation("HttpIntegration Error POST : " + err.Message + ", : Data : " + JsonConvert.SerializeObject(err.Data).ToString());

                throw;
            }

        }

        private void CleanUp()
        {
            Headers = new Dictionary<string, string>();
            Params = new Dictionary<string, string>();
            _httpClient.DefaultRequestHeaders.Clear();
        }

        private string PrepareUrl(string url)
        {

            if (Headers != null)
            {
                foreach (var (key, value) in Headers)
                {
                    _httpClient.DefaultRequestHeaders.Add(key, value);
                }
            }

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            if (Params != null)
            {
                foreach (var (key, value) in Params)
                {
                    queryString.Add(key, value);
                }
            }

            var builder = new UriBuilder(url)
            {
                Query = queryString.ToString()
            };

            return builder.ToString();
        }
    }
}
