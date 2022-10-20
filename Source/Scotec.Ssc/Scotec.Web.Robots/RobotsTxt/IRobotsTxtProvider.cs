namespace Scotec.Web.Robots.RobotsTxt
{
    public interface IRobotsTxtProvider
    {
        Task<RobotsTxtResult> GetResultAsync(CancellationToken cancellationToken);
    }
}
