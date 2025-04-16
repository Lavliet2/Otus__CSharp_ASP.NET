using System;
using System.Threading.Tasks;
using Pcf.Shared.Events;

namespace Pcf.GivingToCustomer.Core.Abstractions.Services
{
    public interface IPromoCodeService
    {
        Task GivePromoCodeToCustomersAsync(PromoCodeReceivedEvent message);
    }
}
