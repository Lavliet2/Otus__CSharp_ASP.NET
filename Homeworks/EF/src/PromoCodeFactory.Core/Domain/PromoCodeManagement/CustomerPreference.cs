﻿using PromoCodeFactory.Core.Domain;
using System;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class CustomerPreference : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }

        public Guid PreferenceId { get; set; }
        public Preference Preference { get; set; }
    }
}
