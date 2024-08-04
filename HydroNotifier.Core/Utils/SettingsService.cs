namespace HydroNotifier.Core.Utils;

public class SettingsService : ISettingsService
{
    public string EmailTo => Environment.GetEnvironmentVariable("EmailTo");
    public string SendGridSenderIdentityName => Environment.GetEnvironmentVariable("SendGridSenderIdentityName");
    public string SendGridSenderIdentityEmail => Environment.GetEnvironmentVariable("SendGridSenderIdentityEmail");
    public string SmsApiKey => Environment.GetEnvironmentVariable("VonageApiKey");
    public string SmsApiSecret => Environment.GetEnvironmentVariable("VonageApiSecret");
    public string SmsTo => Environment.GetEnvironmentVariable("SmsTo");
    public string TableStorageConnectionString => Environment.GetEnvironmentVariable("TableStorageConnectionString");
}