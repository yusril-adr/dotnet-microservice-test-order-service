using System.Text.Json;

namespace DotNetService.Infrastructure.Helpers
{
  public static class JsonSerializeSeeder
  {
    public static JsonSerializerOptions options { get; set; } = new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
      PropertyNameCaseInsensitive = true
    };
  }
}