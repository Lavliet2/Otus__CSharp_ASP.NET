using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pcf.GivingToCustomer.Integration;
using PreferenceEntity = Pcf.GivingToCustomer.Core.Domain.Preference;

namespace Pcf.GivingToCustomer.IntegrationTests.Fakes
{
    public class FakePreferenceGateway : PreferenceGateway
    {
        private readonly List<PreferenceEntity> _preferences;

        public FakePreferenceGateway(List<PreferenceEntity> preferences)
            : base(null!)
        {
            _preferences = preferences;
        }

        public override Task<List<PreferenceEntity>> GetPreferencesAsync()
        {
            return Task.FromResult(_preferences);
        }
    }
}
