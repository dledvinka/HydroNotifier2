namespace HydroNotifier.Core
{
    public class SettingsService : ISettingsService
    {
        public string SmsApiKey => System.Environment.GetEnvironmentVariable("NexmoApiKey");
        public string SmsApiSecret => System.Environment.GetEnvironmentVariable("NexmoApiSecret");
        public string SmsTo => System.Environment.GetEnvironmentVariable("SmsTo");
    }
}
