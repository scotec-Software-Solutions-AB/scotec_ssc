namespace Scotec.Web.Robots.RobotsTxt;

public class RobotsTxtProvider : IRobotsTxtProvider
{
    private readonly Action<RobotsTxtBuilder> _func;

    public RobotsTxtProvider(Action<RobotsTxtBuilder> func)
    {
        _func = func;
    }

    public async Task<RobotsTxtResult> GetResultAsync(CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            var builder = new RobotsTxtBuilder();
            _func(builder);

            var text = builder.Build();

            return Task.FromResult(new RobotsTxtResult(text, builder.MaxAge));
        }, cancellationToken);
    }
}