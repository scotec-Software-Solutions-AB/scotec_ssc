using System.Text;


namespace Scotec.Web.Robots.RobotsTxt
{
    public sealed class RobotsTxtResult
    {
        public RobotsTxtResult(StringBuilder content, TimeSpan maxAge)
        {
            Content = Encoding.UTF8.GetBytes(content.ToString()).AsMemory();
            MaxAge = maxAge;
        }

        public TimeSpan MaxAge { get; }

        public Memory<byte> Content { get; }
    }
}
