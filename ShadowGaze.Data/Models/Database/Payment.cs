namespace ShadowGaze.Data.Models.Database;

public class Payment : BaseDatabaseModel
{
    public int? CustomerId { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Currency {get; set;}
    public int TotalAmount { get; set; }
    public string InvoicePayload {get; set;}
    public string ProviderPaymentChargeId  {get; set;}
    public string TelegramPaymentChargeId   {get; set;}
}