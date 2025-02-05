namespace HillMetrics.MIND.Infrastructure
{
    public class KeycloakConfigMind : Core.Authentication.Keycloak.KeycloakConfig
    {
        public IdentityProviderSettings Azure { get; set; } = null!;

        public override string GetAudience()
        {
            return Azure.ClientId;
        }
    }
}
