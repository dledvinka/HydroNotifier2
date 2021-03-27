namespace HydroNotifier.FunctionApp.Utils
{
    public interface ISettingsService
    {
        string SmsApiKey { get; }
        string SmsApiSecret { get; }
        string SmsTo { get; }
        string EmailTo { get; }
    }
}
