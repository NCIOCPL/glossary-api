using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Defines the configuration for the Glossary API
    /// </summary>    
    public class Startup
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NCI.OCPL.Api.Glossary.Startup"/> class.
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// member variable
        /// </summary>
        public IConfiguration Configuration { get; }
        
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            .AddJsonOptions(options =>
            {
                // Use camel case properties in the serializer and the spec (optional)
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                // Use string enums in the serializer and the spec (optional)
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
            
            // Register the Swagger services
            // Add OpenAPI/Swagger document
            services.AddOpenApiDocument(document =>
            {
                document.Title = "Glossory Term API";
                document.DocumentName = "v1";
                document.PostProcess = document2 => {
                    document2.Host = null;
                    document2.Servers.Clear();
                };
            });
        }

        
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>        
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

            app.UseStaticFiles();

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}