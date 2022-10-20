using System.Text;

namespace Scotec.Web.Robots.RobotsTxt
{
    public class RobotsTxtBuilder
    {
        private List<string> _siteMapUrls = new();
        private List<Action<UserAgentOptionsBuilder>> _actions = new();
        
        public int? CrawlDelay { get; set; }
        
        public  TimeSpan MaxAge { get; private set; } = TimeSpan.FromDays(1);

        public RobotsTxtBuilder AddUserAgent(Action<UserAgentOptionsBuilder> action)
        {
            _actions.Add(action);
            return this;
        }

        public RobotsTxtBuilder AddSiteMapUrl(string siteMapUrl)
        {
            _siteMapUrls.Add(siteMapUrl);
            return this;
        }

        public RobotsTxtBuilder SetMaxAge(TimeSpan maxAge)
        {
            MaxAge = maxAge;
            return this;
        }


        public StringBuilder Build()
        {
            var robotsTxt = new StringBuilder();
            foreach (var action in _actions)
            {
                var optionBuilder = new UserAgentOptionsBuilder();
                action(optionBuilder);

                robotsTxt.AppendLine(optionBuilder.Build());
            }

            foreach(var url in _siteMapUrls)
            {
                robotsTxt.AppendLine($"Sitemap: {url}");
            }

            return robotsTxt;
        }
    }
}
