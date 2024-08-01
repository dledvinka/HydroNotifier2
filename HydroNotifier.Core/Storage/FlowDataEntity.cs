namespace HydroNotifier.Core.Storage;

using Microsoft.Azure.Cosmos.Table;

public class FlowDataEntity : TableEntity
{
    public string EmailNotificationJson { get; set; }
    public bool EmailNotificationSent { get; set; }
    public double LomnaFlowLitersPerSecond { get; set; }
    public string NexmoRemainingBalanceEur { get; set; }
    public double OlseFlowLitersPerSecond { get; set; }
    public string SmsNotificationJson { get; set; }
    public bool SmsNotificationSent { get; set; }
    public string Status { get; set; }
}