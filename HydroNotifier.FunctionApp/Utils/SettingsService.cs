namespace HydroNotifier.FunctionApp.Utils
{
    public class SettingsService : ISettingsService
    {
        public string SmsApiKey => System.Environment.GetEnvironmentVariable("NexmoApiKey");
        public string SmsApiSecret => System.Environment.GetEnvironmentVariable("NexmoApiSecret");
        public string SmsTo => System.Environment.GetEnvironmentVariable("SmsTo");
        public string EmailTo => System.Environment.GetEnvironmentVariable("EmailTo");
        public string TableStorageConnectionString=> System.Environment.GetEnvironmentVariable("TableStorageConnectionString");
    }
}
