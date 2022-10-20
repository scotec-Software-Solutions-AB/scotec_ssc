using Microsoft.Extensions.DependencyInjection;

namespace Scotec.Web.Robots.RobotsTxt
{
    public static class IServiceCollectionExtensions
    {
        public static void AddRobotsTxt(this IServiceCollection services, Action<RobotsTxtBuilder> action)
        {
            services.AddSingleton<IRobotsTxtProvider>(new RobotsTxtProvider(action));
        }
    }
}
