using System;
using System.Collections.Generic;
using System.Text;

namespace DCE.Security
{
    public static class OAuthSettings
    {
        public static string Scopes = "User.Read User.ReadBasic.All Files.ReadWrite.All Group.Read.All Sites.ReadWrite.All offline_access";
        public static string ApplicationId = "76d20872-9659-436a-a9af-3e627b3ae352";
        public static string RedirectUri = "msal76d20872-9659-436a-a9af-3e627b3ae352://auth";
        public static string TenantId = "04a45e0f-9cb5-481c-9f99-b93142e90a6d";
    }
}
