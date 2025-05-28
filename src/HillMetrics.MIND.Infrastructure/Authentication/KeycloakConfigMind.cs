namespace HillMetrics.MIND.Infrastructure
{
    public class KeycloakConfigMind : Core.Authentication.Keycloak.KeycloakConfig
    {
        public IdentityProviderSettings Azure { get; set; } = null!;

        public ClientCredentialsSettings Private { get; set; } = null!;

        public override string GetClientId()
        {
            return Azure.ClientId;
        }

        public override string GetClientSecret()
        {
            return Azure.ClientSecret;
        }

        public override string[] GetValidAudiences()
        {
            if (string.IsNullOrEmpty(Audiences))
                return [Azure.ClientId];

            return Audiences.Split(';', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
