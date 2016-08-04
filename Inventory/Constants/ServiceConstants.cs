namespace Inventory.Constants
{
    public class ServiceConstants
    {
        public const string MSA_CLIENT_ID = "sorry";
        public const string MSA_CLIENT_SECRET = "not telling";

        /// <summary>
        /// Return url for the OneDrive authentication
        /// </summary>
        public const string RETURN_URL = "https://login.live.com/oauth20_desktop.srf";

        /// <summary>
        /// Authentication url for the OneDrive authentication
        /// </summary>
        public const string AUTHENTICATION_URL = "https://login.live.com/oauth20_authorize.srf";

        /// <summary>
        /// The Token URL is used to retrieve a access token in the code flow oauth
        /// </summary>
        public const string TOKEN_URL = "https://login.live.com/oauth20_token.srf";

        /// <summary>
        /// Scopes for OneDrive access
        /// </summary>
        public static string[] Scopes = { "onedrive.readwrite", "wl.offline_access", "wl.signin", "onedrive.readonly" };
    }
}
