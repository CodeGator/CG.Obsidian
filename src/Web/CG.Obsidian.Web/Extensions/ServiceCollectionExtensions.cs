using CG.Blazor;
using CG.DataProtection;
using CG.Obsidian.Web.Options;
using CG.Validations;
using CG.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.OpenApi.Models;

namespace CG.Obsidian.Web.Extensions
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IServiceCollection"/>
    /// type.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method adds custom Blazor logic for the server.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/> parameter,
        /// for chaining calls together.</returns>
        public static IServiceCollection AddCustomBlazor(
            this IServiceCollection serviceCollection,
            IConfiguration configuration
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Add support for razor pages.
            serviceCollection.AddRazorPages();

            // Add support for server-side Blazor.
            serviceCollection.AddServerSideBlazor();

            // Add support for controllers.
            serviceCollection.AddControllersWithViews();

            // Add the HTTP client.
            serviceCollection.AddHttpClient();

            // Return the service collection.
            return serviceCollection;
        }

        // *******************************************************************

        /// <summary>
        /// This method adds custom identity logic for the server.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/> parameter,
        /// for chaining calls together.</returns>
        public static IServiceCollection AddCustomIdentity(
            this IServiceCollection serviceCollection,
            IConfiguration configuration
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Add the token provider.
            serviceCollection.AddTokenProvider(
                configuration,
                ServiceLifetime.Scoped
                );

            // Configure the options.
            serviceCollection.ConfigureOptions(
                configuration,
                out IdentityOptions identityOptions
                );

            // Unprotect any protected properties.
            DataProtector.Instance().UnprotectProperties(
                identityOptions
                );

            // Add bearer token authentication for the internal API.
            serviceCollection.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    options.Authority = identityOptions.Authority;
                    options.ApiName = identityOptions.API.ApiName;
                    options.ApiSecret = identityOptions.API.ApiSecret;
                    options.SaveToken = true;
                });

            // Add an authentication policy for use with the internal API.
            var requireAuthenticatedUserPolicy = new AuthorizationPolicyBuilder()
                  .RequireAuthenticatedUser()
                  .Build();

            // Add cookie based openid authentication for the UI.
            serviceCollection.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = identityOptions.Authority;
                options.ClientId = identityOptions.OIDC.ClientId;
                options.ClientSecret = identityOptions.OIDC.ClientSecret;
                options.ResponseType = "code";
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("offline_access");
                options.Scope.Add("obsidianapi.read");
                options.Scope.Add("obsidianapi.write");
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.TokenValidationParameters.NameClaimType = "given_name";
            });

            // Return the service collection.
            return serviceCollection;
        }

        // *******************************************************************

        /// <summary>
        /// This method adds custom CG.Obsidian logic for the server.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/> parameter,
        /// for chaining calls together.</returns>
        public static IServiceCollection AddCustomObsidian(
            this IServiceCollection serviceCollection,
            IConfiguration configuration
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Add the stores.
            serviceCollection.AddObsidianStores(
                configuration.GetSection("Stores"),
                ServiceLifetime.Singleton
                );

            // Add the managers.
            serviceCollection.AddObsidianManagers(
                configuration.GetSection("Managers"),
                ServiceLifetime.Singleton
                );

            // Add the data layer.
            serviceCollection.AddRepositories(
                configuration.GetSection("Repositories"), 
                ServiceLifetime.Singleton
                );

            // Return the service collection.
            return serviceCollection;
        }

        // *******************************************************************

        /// <summary>
        /// This method adds custom swagger documentation for the server.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/> parameter,
        /// for chaining calls together.</returns>
        public static IServiceCollection AddCustomSwagger(
            this IServiceCollection serviceCollection,
            IConfiguration configuration
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Wire up Swagger.
            serviceCollection.AddSwaggerGen(c =>
            {
                // Add swagger docs.
                c.SwaggerDoc(
                    "v1", 
                    new OpenApiInfo 
                    { 
                        Title = typeof(Program).Assembly.ReadProduct(),
                        Description = typeof(Program).Assembly.ReadDescription(),
                        Version = "v1" 
                    });
            });

            // Return the service collection.
            return serviceCollection;
        }

        // *******************************************************************

        /// <summary>
        /// This method adds custom health checks for the server.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/> parameter,
        /// for chaining calls together.</returns>
        public static IServiceCollection AddCustomHealthChecks(
            this IServiceCollection serviceCollection,
            IConfiguration configuration
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Add the health checks.
            serviceCollection.AddHealthChecks()
                .AddSqlServer(
                    configuration["CG.Obsidian:Repositories:SqlServer:ConnectionString"]
                    );
                
            // Add health checks UI.
            serviceCollection.AddHealthChecksUI(s =>
            {
                s.AddHealthCheckEndpoint(typeof(Program).Assembly.ReadProduct(), "/hc");
                s.SetHeaderText($"{typeof(Program).Assembly.ReadProduct()} Health");
            }).AddInMemoryStorage();

            // Return the service collection.
            return serviceCollection;
        }

        #endregion
    }
}
