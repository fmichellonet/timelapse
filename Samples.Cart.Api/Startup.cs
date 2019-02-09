namespace Samples.Cart.Api
{
	using Domain;
	using MediatR;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.WindowsAzure.Storage;
	using Microsoft.WindowsAzure.Storage.Table;
	using Swashbuckle.AspNetCore.Swagger;
	using Timelapse;
	using Timelapse.Domain;
	using Timelapse.EventPublisher.MediatR;
	using Timelapse.Persistence;
	using Timelapse.Persistence.Streamstone;

	public class Startup
	{
		private readonly AppSettings _appSettings;

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
			_appSettings = new AppSettings();
			configuration.Bind(_appSettings);
        }

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			ConfigureSwagger(services);
			ConfigureDependencies(services);
		}

		private void ConfigureDependencies(IServiceCollection services)
		{
			var storageAccount = CloudStorageAccount.Parse(_appSettings.StorageConnectionString);
			var tableClient = storageAccount.CreateCloudTableClient();

			services.AddTransient<CloudTable>(provider => tableClient.GetTableReference("CartEvents"));
            services.AddTransient<IEventStore, EventStore>();
            services.AddTransient<IRepository<Cart, CartId>, EventSourcingRepository<Cart, CartId>>();

			services.AddTransient<ServiceFactory>(provider => provider.GetService);
            services.AddTransient<IMediator, Mediator>();
            services.AddTransient<ITransientDomainEventPublisher, EventPublisher>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			UseSwagger(app);
			app.UseHttpsRedirection();
			app.UseMvcWithDefaultRoute();
		}

		private void ConfigureSwagger(IServiceCollection services)
		{
			// Register the Swagger generator, defining 1 or more Swagger documents
			services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info {Title = "Samples.Cart", Version = "v1"}); });
		}

		private void UseSwagger(IApplicationBuilder app)
		{
			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Samples.Cart");
				c.RoutePrefix = string.Empty;
			});
		}
	}
}