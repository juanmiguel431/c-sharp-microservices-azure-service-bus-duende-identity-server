using Mango.MessageBus;

namespace Mango.Services.PaymentApi.Messages;

public class UpdatePaymentResultMessage : BaseMessage
{
    public int OrderId { get; set; }
    public bool Status { get; set; }
}