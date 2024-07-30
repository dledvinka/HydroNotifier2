namespace HydroNotifier.Core.Utils;

public class SettingsService : ISettingsService
{
    public string EmailTo => Environment.GetEnvironmentVariable("EmailTo");
    public string SendGridSenderIdentityName => Environment.GetEnvironmentVariable("SendGridSenderIdentityName");
    public string SendGridSenderIndentityEmail => Environment.GetEnvironmentVariable("SendGridSenderIndentityEmail");
    public string SmsApiKey => Environment.GetEnvironmentVariable("NexmoApiKey");
    public string SmsApiSecret => Environment.GetEnvironmentVariable("NexmoApiSecret");
    public string SmsTo => Environment.GetEnvironmentVariable("SmsTo");
    public string TableStorageConnectionString => Environment.GetEnvironmentVariable("TableStorageConnectionString");
}