namespace HydroNotifier.Core.Utils;

public interface ISettingsService
{
    string EmailTo { get; }
    string SendGridSenderIdentityName { get; }
    string SendGridSenderIdentityEmail { get; }
    string SmsApiKey { get; }
    string SmsApiSecret { get; }
    string SmsApiKey2 { get; }
    string SmsApiSecret2 { get; }
    string SmsTo { get; }
    string TableStorageConnectionString { get; }
}