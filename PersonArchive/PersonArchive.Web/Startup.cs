using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonArchive.Entities;
using PersonArchive.Web.DataContexts;
using PersonArchive.Web.Services;

namespace PersonArchive.Web
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<PersonDbContext>(
				options =>
					options.UseSqlServer(Configuration.GetConnectionString("PersonArchiveDatabaseConnectionString")));

			services.Configure<DataTripleStoreServiceSettingsModel>(Configuration.GetSection("DataTripleStoreServiceSettings"));

			/*
			* TODO change to query key in azure portal
			* in the part of your application that executes queries,
			* it is better to create the SearchIndexClient directly so
			* that you can pass in a query key instead of an admin key.
			* This is consistent with the principle of least privilege
			* and will help to make your application more secure.
			*/
			services.Configure<SearchStoreServiceSettingsModel>(Configuration.GetSection("SearchStoreServiceSettings"));

			services.AddScoped<IPersonData, SqlPersonData>();
			services.AddScoped<DataTripleStore.Services.IPersonData, DataTripleStore.Services.SparqlPersonData>();
			services.AddScoped<SearchStore.Services.IPersonIndex, SearchStore.Services.PersonIndex>();

			services.AddMvc();

			// Require HTTPS in production (settings)
			services.AddHttpsRedirection(options =>
			{
				options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
				options.HttpsPort = 443;
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(
			IApplicationBuilder app,
			IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");

				// Require HTTPS in production
				app.UseHttpsRedirection();
			}

			app.UseStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					"default",
					"{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}