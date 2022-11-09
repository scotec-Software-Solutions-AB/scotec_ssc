namespace Scotec.Web.Robots.RobotsTxt;

public abstract class UserAgentOption
{
    protected UserAgentOption(string content)
    {
        Content = content;
    }

    public string Content { get; set; }

    protected abstract string GetPrefix();

    public override string ToString()
    {
        return $"{GetPrefix()} {Content}".Trim();
    }
}

public class UserAgentNameOption : UserAgentOption
{
    public UserAgentNameOption(string name) : base(name)
    {
    }

    protected override string GetPrefix()
    {
        return "User-agent:";
    }
}

public class UserAgentCrawlDelayOption : UserAgentOption
{
    public UserAgentCrawlDelayOption(int crawlDelay) : base(crawlDelay.ToString())
    {
    }

    protected override string GetPrefix()
    {
        return "Crawl-delay:";
    }
}

public class UserAgentCommentOption : UserAgentOption
{
    public UserAgentCommentOption(string comment) : base(comment)
    {
    }

    protected override string GetPrefix()
    {
        return "#";
    }
}

public class UserAgentAllowOption : UserAgentOption
{
    public UserAgentAllowOption(string allow) : base(allow)
    {
    }

    protected override string GetPrefix()
    {
        return "Allow:";
    }
}

public class UserAgentDisallowOption : UserAgentOption
{
    public UserAgentDisallowOption(string disallow) : base(disallow)
    {
    }

    protected override string GetPrefix()
    {
        return "Disallow:";
    }
}

public class UserAgentEmptyLineOption : UserAgentOption
{
    public UserAgentEmptyLineOption() : base(string.Empty)
    {
    }

    protected override string GetPrefix()
    {
        return string.Empty;
    }
}