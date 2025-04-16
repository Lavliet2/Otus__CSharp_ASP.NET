namespace Pcf.Shared.Events;

public record PromoCodeReceivedEvent(
    string PartnerName,
    string PromoCode,
    Guid? ManagerId
);
