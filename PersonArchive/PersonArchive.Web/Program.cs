using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PersonArchive.Web.DataContexts;

namespace PersonArchive.Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = BuildWebHost(args);

			using (var scope = host.Services.CreateScope())
			{
				var services =
					scope.ServiceProvider;

				var context =
					services.GetRequiredService<PersonDbContext>();

				DbInitializer.Seed(context);
			}

			host.Run();
		}

		public static IWebHost BuildWebHost(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.Build();
		}
	}
}