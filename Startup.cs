using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Http;

namespace TesteRedisStateSession
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
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => false; // true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddControllersWithViews();

			services.AddStackExchangeRedisCache(options =>
			{
				options.Configuration = Configuration.GetConnectionString("CacheConnection");
				options.InstanceName = "SampleInstance";
			});

			//Add services needed for sessions
			services.AddSession();
		}


		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		//public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		//{
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, IDistributedCache cache)
		{
			lifetime.ApplicationStarted.Register(() =>
			{
				var currentTimeUTC = DateTime.UtcNow.ToString();
				byte[] encodedCurrentTimeUTC = Encoding.UTF8.GetBytes(currentTimeUTC);
				var options = new DistributedCacheEntryOptions()
					.SetSlidingExpiration(TimeSpan.FromSeconds(20));
				cache.Set("cachedTimeUTC", encodedCurrentTimeUTC, options);
			});

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.UseSession();

			app.UseCookiePolicy();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
