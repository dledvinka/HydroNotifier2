namespace HydroNotifier.Core.Utils;

public class SettingsService : ISettingsService
{
    public string EmailTo => Environment.GetEnvironmentVariable("EmailTo");
    public string SendGridSenderIdentityName => Environment.GetEnvironmentVariable("SendGridSenderIdentityName");
    public string SendGridSenderIdentityEmail => Environment.GetEnvironmentVariable("SendGridSenderIdentityEmail");
    public string SmsApiKey => Environment.GetEnvironmentVariable("NexmoApiKey");
    public string SmsApiSecret => Environment.GetEnvironmentVariable("NexmoApiSecret");
    public string SmsApiKey2 => Environment.GetEnvironmentVariable("VonageApiKey");
    public string SmsApiSecret2 => Environment.GetEnvironmentVariable("VonageApiSecret");
    public string SmsTo => Environment.GetEnvironmentVariable("SmsTo");
    public string TableStorageConnectionString => Environment.GetEnvironmentVariable("TableStorageConnectionString");
}