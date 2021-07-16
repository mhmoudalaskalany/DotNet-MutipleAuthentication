using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MultipleAccessToken.Extension;

namespace MultipleAccessToken
{
    /// <summary>
    /// Dependency Extensions
    /// </summary>
    public static class DependencyExtension
    {
        /// <summary>
        /// Add Swagger Config
        /// </summary>
        /// <param name="services"></param>
        /// <param name="xmlFilePath"></param>
        /// <param name="azureAdTenantId"></param>
        
        public static void AddSwaggerConfig(this IServiceCollection services, string xmlFilePath, string azureAdTenantId)
        {

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My API",
                    Version = "v1"
                });

                //c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });


                c.IncludeXmlComments(xmlFilePath);
                c.OperationFilter<AddRequiredHeaderParameter>();
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {

                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{azureAdTenantId}/oauth2/authorize"),
                            TokenUrl = new Uri($"https://login.microsoftonline.com/{azureAdTenantId}/oauth2/v2.0/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                //{"user_impersonation", "Access API"},
                                {"access_as_user", "Access API"},
                                {"api://1dcbf44d-867d-4c19-b9f1-7bc7370c534b/default", "default"}
                            }
                        }
                    }// "implicit",

                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                        }, new[] { "access_as_user" }
                    }
                });

            });
        }

        /// <summary>
        /// User Swagger Middle Ware
        /// </summary>
        /// <param name="app"></param>
        /// <param name="swaggerClientId"></param>
        /// <param name="swaggerClientSecret"></param>
        /// <param name="azureAdClientId"></param>
        /// <param name="apiName"></param>
        /// <param name="endPoint"></param>
        public static void UseSwaggerMiddleware(this IApplicationBuilder app, string swaggerClientId,
            string swaggerClientSecret, string azureAdClientId, string apiName, string endPoint)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.OAuthClientId(swaggerClientId);
                c.OAuthClientSecret(swaggerClientSecret);
                c.OAuthRealm(azureAdClientId);
                c.OAuthAppName("My API V1");
                c.OAuthScopeSeparator(" ");
                c.OAuthAdditionalQueryStringParams(new Dictionary<string, string>()
                    {{"resource", azureAdClientId}});

                c.SwaggerEndpoint(endPoint/*"/swagger/v1/swagger.json"*/, apiName);
            });
        }
    }
}