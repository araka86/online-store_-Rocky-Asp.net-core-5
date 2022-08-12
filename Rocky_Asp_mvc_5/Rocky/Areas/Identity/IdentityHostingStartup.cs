using Microsoft.AspNetCore.Hosting;
using Rocky2.Areas.Identity;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]
namespace Rocky2.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}
