using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransactionStatus
{
    PENDING,
    COMPLETED
}
public class TransactionDto{
    public Guid UserId { get; set; }
    public Guid TransactionId { get; set; }
    public int RequiredTokens { get; set; }
    public int NumberOfDigits { get; set; }
    public TransactionStatus Status { get; set; }
}