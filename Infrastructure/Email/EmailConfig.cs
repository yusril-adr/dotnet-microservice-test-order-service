namespace DotnetOrderService.Infrastructure.Email
{
    public class EmailConfig
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public bool UseAuthentication { get; set; }
        public bool UseSsl { get; set; }
        public string SenderName { get; set; }
        public string SenderAddress { get; set; }

        public static EmailConfig ParseFromConfiguration(IConfiguration configuration)
        {
            return new EmailConfig
            {
                SmtpServer = configuration.GetValue<string>("Email:SmtpServer"),
                SmtpPort = configuration.GetValue<int>("Email:SmtpPort"),
                SmtpUsername = configuration.GetValue<string>("Email:SmtpUsername"),
                SmtpPassword = configuration.GetValue<string>("Email:SmtpPassword"),
                UseAuthentication = configuration.GetValue<bool>("Email:UseAuthentication"),
                UseSsl = configuration.GetValue<bool>("Email:UseSsl"),
                SenderName = configuration.GetValue<string>("Email:SenderName"),
                SenderAddress = configuration.GetValue<string>("Email:SenderAddress")
            };
        }
    }
}