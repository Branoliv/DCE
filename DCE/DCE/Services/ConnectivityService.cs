using DCE.Services.Interface;
using System;
using System.Linq;
using Xamarin.Essentials;

namespace DCE.Services
{
    public class ConnectivityService : IConnectivityService
    {
        public bool VerifyInternet { get; private set; }
        public bool IsNetworkAccess()
        {
            var networkAccess = Connectivity.NetworkAccess;

            if (networkAccess == NetworkAccess.Internet)
            {
                var profiles = Connectivity.ConnectionProfiles;

                //TODO - conexão com wifi
                if (!profiles.Contains(ConnectionProfile.WiFi))
                {

                }

                VerifyInternet = true;
            }
            else
            {
                VerifyInternet = false;
            }

            return VerifyInternet;
        }
    }
}
