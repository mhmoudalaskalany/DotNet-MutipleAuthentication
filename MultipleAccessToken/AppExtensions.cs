using Microsoft.AspNetCore.Builder;

namespace MultipleAccessToken
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class AppExtensions
    {
        /// <summary>
        /// Cors
        /// </summary>
        /// <param name="app"></param>
        public static void ConfigureCors(this IApplicationBuilder app)
        {
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        }
    }
}