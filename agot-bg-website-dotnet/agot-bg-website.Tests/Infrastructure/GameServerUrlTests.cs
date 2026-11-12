using agot_bg_website.Infrastructure;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace agot_bg_website.Tests.Infrastructure;

public class GameServerUrlTests
{
    private static IConfiguration Config(params (string Key, string Value)[] values) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(values.Select(v => new KeyValuePair<string, string?>(v.Key, v.Value)))
            .Build();

    [Fact]
    public void ConfiguredUrl_Wins()
    {
        var config = Config(
            ("PublicSiteUrl", "https://got.example.com"),
            ("GameServerUrl", "wss://got-play.example.com")
        );

        Assert.Equal("wss://got-play.example.com", GameServerUrl.FromConfig(config));
    }

    [Fact]
    public void Unset_FallsBackToPlaySubdomain()
    {
        var config = Config(("PublicSiteUrl", "https://swordsandravens.net"));

        Assert.Equal("wss://play.swordsandravens.net", GameServerUrl.FromConfig(config));
    }

    [Fact]
    public void Localhost_UsesLocalGameServer()
    {
        Assert.Equal("ws://localhost:5000", GameServerUrl.FromConfig(Config()));
    }
}
