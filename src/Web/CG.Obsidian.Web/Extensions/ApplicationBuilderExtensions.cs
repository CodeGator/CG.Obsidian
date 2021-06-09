using CG.Reflection;
using CG.Validations;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace CG.Obsidian.Web.Extensions
{
    /// <summary>
     /// This class contains extension methods related to the <see cref="IApplicationBuilder"/>
     /// type.
     /// </summary>
    public static partial class ApplicationBuilderExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method wires up any startup logic required, by Blazor, to support
        /// the server.
        /// </summary>
        /// <param name="applicationBuilder">The application builder to use for
        /// the operation.</param>
        /// <param name="webHostEnvironment">The web host environment to use 
        /// for the operation.</param>
        /// <returns>The value of the <paramref name="applicationBuilder"/> parameter,
        /// for chaining calls together.</returns>
        public static IApplicationBuilder UseCustomBlazor(
            this IApplicationBuilder applicationBuilder,
            IWebHostEnvironment webHostEnvironment
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(applicationBuilder, nameof(applicationBuilder))
                .ThrowIfNull(webHostEnvironment, nameof(webHostEnvironment));

            // Are we running in a development environment?
            if (webHostEnvironment.IsDevelopment())
            {
                // Use the developer error page.
                applicationBuilder.UseDeveloperExceptionPage();
            }
            else
            {
                // Use the standard error handler.
                applicationBuilder.UseExceptionHandler("/Error");

                // Use HSTS
                applicationBuilder.UseHsts();
            }

            // Use HTTPS
            applicationBuilder.UseHttpsRedirection();

            // Use static files.
            applicationBuilder.UseStaticFiles();

            // Use routing.
            applicationBuilder.UseRouting();

            // Use authentication.
            applicationBuilder.UseAuthentication();

            // Use authorization.
            applicationBuilder.UseAuthorization();

            // Use endpoint routing.
            applicationBuilder.UseEndpoints(endpoints =>
            {
                // Map the Blazor hub.
                endpoints.MapBlazorHub();

                // Map the fallback page.
                endpoints.MapFallbackToPage("/_Host");

                // Map the default controller route.
                endpoints.MapDefaultControllerRoute();
            });

            // Return the application builder.
            return applicationBuilder;
        }

        // *******************************************************************

        /// <summary>
        /// This method wires up any startup logic required, by CG.Obsidian, 
        /// to support the server.
        /// </summary>
        /// <param name="applicationBuilder">The application builder to use for
        /// the operation.</param>
        /// <param name="webHostEnvironment">The web host environment to use 
        /// for the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <returns>The value of the <paramref name="applicationBuilder"/> parameter,
        /// for chaining calls together.</returns>
        public static IApplicationBuilder UseCustomObsidian(
            this IApplicationBuilder applicationBuilder,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(applicationBuilder, nameof(applicationBuilder))
                .ThrowIfNull(webHostEnvironment, nameof(webHostEnvironment))
                .ThrowIfNull(configuration, nameof(configuration));

            // Setup the data layer.
            applicationBuilder.UseRepositories(
                webHostEnvironment,
                configuration.GetSection("Repositories")
                );

            // Return the application builder.
            return applicationBuilder;
        }

        // *******************************************************************

        /// <summary>
        /// This method wires up any startup logic required, by Swagger, to 
        /// support the server.
        /// </summary>
        /// <param name="applicationBuilder">The application builder to use for
        /// the operation.</param>
        /// <param name="webHostEnvironment">The web host environment to use 
        /// for the operation.</param>
        /// <returns>The value of the <paramref name="applicationBuilder"/> parameter,
        /// for chaining calls together.</returns>
        public static IApplicationBuilder UseCustomSwagger(
            this IApplicationBuilder applicationBuilder,
            IWebHostEnvironment webHostEnvironment
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(applicationBuilder, nameof(applicationBuilder))
                .ThrowIfNull(webHostEnvironment, nameof(webHostEnvironment));

            // Add swagger.
            applicationBuilder.UseSwagger();

            // Add swagger UI.
            applicationBuilder.UseSwaggerUI(c => 
                c.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    $"{typeof(Program).Assembly.ReadProduct()} v1"
                    ));

            // Return the application builder.
            return applicationBuilder;
        }

        // *******************************************************************

        /// <summary>
        /// This method wires up any startup logic required, by health checks, 
        /// to support the server.
        /// </summary>
        /// <param name="applicationBuilder">The application builder to use for
        /// the operation.</param>
        /// <param name="webHostEnvironment">The web host environment to use 
        /// for the operation.</param>
        /// <returns>The value of the <paramref name="applicationBuilder"/> parameter,
        /// for chaining calls together.</returns>
        public static IApplicationBuilder UseCustomHealthChecks(
            this IApplicationBuilder applicationBuilder,
            IWebHostEnvironment webHostEnvironment
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(applicationBuilder, nameof(applicationBuilder))
                .ThrowIfNull(webHostEnvironment, nameof(webHostEnvironment));

            // Wire up health checks.
            applicationBuilder.UseHealthChecks("/hc");

            // Wire up the health check endpoints.
            applicationBuilder.UseEndpoints(endpoints =>
            {
                // In case we haven't already done this.
                endpoints.MapControllers();

                // Map the health checks UI.
                endpoints.MapHealthChecksUI(settings =>
                {
                    settings.AddCustomStylesheet("wwwroot/css/health.css");
                });

                // Map the health checks.
                endpoints.MapHealthChecks(
                    "/hc", 
                    new HealthCheckOptions()
                    {
                        Predicate = _ => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });
            });

            // Return the application builder.
            return applicationBuilder;
        }

        #endregion
    }
}
