
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DotNetService.Infrastructure.Shareds
{
    public static class Utils
    {
        public static string ToSnakeCase(string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        public static string RandStr(int length)
        {
            var random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string JsonSerialize(object json)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

            return JsonConvert.SerializeObject(json);
        }

        public static T JsonDeserialize<T>(string json)
        {
            var responseJson = JsonConvert.DeserializeObject<T>(json);
            return responseJson;
        }

        public static T JsonDeserializeResponse<T>(HttpContent response)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

            using var reader = new StreamReader(response.ReadAsStream());
            var responseBody = reader.ReadToEnd();
            var responseJson = JsonConvert.DeserializeObject<T>(responseBody.ToString());

            return responseJson;
        }

        public static void BackgroundProcessThreadAsync(Func<Task> func)
        {
            Thread thread = new(async () => { await func(); });
            thread.Start();
        }

        public static void BackgroundProcessThreadSync(Func<bool> func)
        {
            Thread thread = new(() => { func(); });
            thread.Start();
        }

        public static string MoveFileToTemp(IFormFile file, string folderPath)
        {
            string originalName = Path.GetFileName(file.FileName);
            var tempFilePath = "temp/" + folderPath + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + originalName;
            using (FileStream fs = File.Create(tempFilePath))
            {
                file.CopyTo(fs);
            }

            return tempFilePath;
        }

        public static bool IsSuccessStatusCode(int statusCode)
        {
            return statusCode >= 200 && statusCode <= 299;
        }

        public static Dictionary<string, object> SuccessResponseFormat(object data = null)
        {
            return new Dictionary<string, object> {
                { "data", new Dictionary<string, object>{
                    { "success", true },
                    { "data", data },
                }}
            };
        }

        public static Dictionary<string, object> ErrorResponseFormat(string message)
        {
            return new Dictionary<string, object> {
                { "data", new Dictionary<string, object>{
                    { "success", false },
                    { "data", null },
                    { "message", message },
                }}
            };
        }

        public static string MoveFileToStorage(IFormFile file, string storagePath, string folderPath)
        {
            var originalName = Path.GetFileName(file.FileName);
            var filePath = folderPath + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + originalName;
            var storageFilePath = storagePath + "/" + filePath;
            using (FileStream fs = File.Create(storageFilePath))
            {
                file.CopyTo(fs);
            }

            return filePath;
        }

        public static List<string> MoveFilesToTemp(IFormFile[] files, string filePath)
        {
            var tempFilePaths = new List<string>();

            foreach (var file in files)
            {
                string originalName = Path.GetFileName(file.FileName);
                var tempFilePath = "temp/" + filePath + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + originalName;
                using (FileStream fs = File.Create(tempFilePath))
                {
                    file.CopyTo(fs);
                }

                tempFilePaths.Add(tempFilePath);
            }

            return tempFilePaths;
        }

        public static string GetFileExtension(this IFormFile file)
        {
            string originalName = Path.GetFileName(file.FileName);
            return originalName.Split('.').Last();
        }

        public static string GenerateDestPath(this IFormFile fileObject)
        {
            var extension = GetFileExtension(fileObject);
            return "/uploads/" + DateTime.Now.ToString("yyyyMMddHHmmss") + RandStr(10).ToLower() + "-" + "file-uploaded." + extension;
        }

        public static byte[] ParseObjectToByte(Object obj)
        {
            BinaryFormatter bf = new();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T ParseByteToObject<T>(byte[] arrBytes)
        {
            using var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);
            return (T)obj;
        }

        public static int CountPage(int totalData, int take)
        {
            var totalDataDec = decimal.Parse(string.Concat(totalData));

            return (int)Math.Ceiling(totalDataDec / take);
        }

        public static Guid GetUserLoggedId(IHttpContextAccessor httpContextAccessor)
        {
            var userIdClaim = httpContextAccessor.HttpContext.User.FindFirst("id")?.Value;
            if (userIdClaim != null && Guid.TryParse(userIdClaim, out Guid userId))
            {
                return userId;
            }
            return Guid.Empty;
        }
    }
}
