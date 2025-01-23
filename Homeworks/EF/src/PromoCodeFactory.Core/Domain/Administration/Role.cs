using PromoCodeFactory.Core.Domain;
using System;

namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        // Поле дле теста миграций 
        public bool ActiveIND { get; set; } = true;
    }
}