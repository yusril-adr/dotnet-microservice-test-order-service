namespace DotnetOrderService.Infrastructure.Shareds
{
    public static class ResponseUtil
    {
        public static Dictionary<string, object> SuccessFormat(object data = null)
        {
            return new Dictionary<string, object>
            {
                ["data"] = new { success = true, data }
            };
        }

        public static Dictionary<string, object> ErrorFormat(string message)
        {
            return new Dictionary<string, object>
            {
                ["data"] = new { success = false, data = (object)null, message }
            };
        }

    }
}