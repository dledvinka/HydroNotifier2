namespace HydroNotifier.Core
{
    public interface ISettingsService
    {
        string SmsApiKey { get; }
        string SmsApiSecret { get; }
        string SmsTo { get; }
    }
}
