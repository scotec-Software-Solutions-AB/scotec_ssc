using System.Text;

namespace Scotec.Web.Robots.RobotsTxt;

public class UserAgentOptionsBuilder
{
    private readonly List<UserAgentOption> _userOptions = new();

    public string Build()
    {
        if (!_userOptions.OfType<UserAgentNameOption>().Any()) _userOptions.Insert(0, new UserAgentNameOption("*"));

        if (!_userOptions.OfType<UserAgentAllowOption>().Any() && !_userOptions.OfType<UserAgentDisallowOption>().Any())
            _userOptions.Add(new UserAgentAllowOption("/"));

        var robotsTxt = new StringBuilder();
        foreach (var userOption in _userOptions) robotsTxt.AppendLine(userOption.ToString());

        return robotsTxt.ToString();
    }

    public UserAgentOptionsBuilder AddComment(string comment)
    {
        _userOptions.Add(new UserAgentCommentOption(comment));
        return this;
    }

    public UserAgentOptionsBuilder SetUserAgentName(string agentName)
    {
        _userOptions.Add(new UserAgentNameOption(agentName));
        return this;
    }

    public UserAgentOptionsBuilder SetUserAgentCrawlDelay(int crawlDelay)
    {
        _userOptions.Add(new UserAgentCrawlDelayOption(crawlDelay));
        return this;
    }

    public UserAgentOptionsBuilder Allow(string allow)
    {
        _userOptions.Add(new UserAgentAllowOption(allow));
        return this;
    }

    public UserAgentOptionsBuilder Diallow(string disallow)
    {
        _userOptions.Add(new UserAgentDisallowOption(disallow));
        return this;
    }
}