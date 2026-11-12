namespace agot_bg_website.Infrastructure;

public static class GameServerUrl
{
    // Self-hosts set GameServerUrl when play.<host> is unavailable.
    public static string FromConfig(IConfiguration config)
    {
        var configured = config["GameServerUrl"];
        if (!string.IsNullOrEmpty(configured))
        {
            return configured;
        }

        var publicSiteHost = new Uri(config["PublicSiteUrl"] ?? "http://localhost:8000").Host;
        return publicSiteHost == "localhost" ? "ws://localhost:5000" : $"wss://play.{publicSiteHost}";
    }
}
