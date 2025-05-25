namespace HillMetrics.MIND.FrontApp.Configs
{
    public class FeaturesConfig
    {
        public AiChatConfig AiChat { get; set; } = new();
    }

    public class AiChatConfig
    {
        public bool Enabled { get; set; } = false;
    }
}