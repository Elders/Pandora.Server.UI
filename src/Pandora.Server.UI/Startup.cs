using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Elders.Pandora.Server.UI.ViewModels;
using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Authentication.OAuth;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Elders.Pandora.Server.UI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            ApplicationConfiguration.SetContext("Elders.Pandora.Server.UI");

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseCookieAuthentication(options =>
            {
                options.AutomaticAuthenticate = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(50);
            });

            app.UseGoogleAuthentication(options =>
            {
                options.AutomaticChallenge = true;
                options.ClientId = ApplicationConfiguration.Get("client_id");
                options.ClientSecret = ApplicationConfiguration.Get("client_secret");
                options.SignInScheme = "Cookies";

                var scopes = ApplicationConfiguration.Get("scopes").Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach (var scope in scopes)
                {
                    options.Scope.Add(scope);
                }

                options.AccessType = "offline";

                options.SaveTokensAsClaims = true;

                options.Events = new OAuthEvents()
                {
                    OnCreatingTicket = ctx =>
                    {
                        var tokens = ctx.TokenResponse;
                        ctx.Identity.AddClaim(new Claim("id_token", tokens.Response["id_token"].ToString()));
                        ctx.Identity.AddClaim(new Claim("avatar", ctx.User["image"]["url"].ToString()));
                        return Task.CompletedTask;
                    }
                };
            });

            app.UseClaimsTransformation(new ClaimsTransformationOptions() { Transformer = new ClaimsTransformer() });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "UsersRout",
                    template: "Users/{userId}",
                    defaults: new { controller = "Users", action = "Edit" }
                );

                routes.MapRoute(
                    name: "DefaultsRoute",
                    template: "Projects/{projectName}/{applicationName}/Clusters/Defaults",
                    defaults: new { controller = "Clusters", action = "Defaults" }
                );

                routes.MapRoute(
                    name: "AddMachineRoute",
                    template: "Projects/{projectName}/{applicationName}/{clusterName}/Machines/AddMachine",
                    defaults: new { controller = "Machines", action = "AddMachine" }
                );

                routes.MapRoute(
                    name: "AplicationRoute",
                    template: "Projects/{projectName}/{applicationName}/{clusterName}/Machines",
                    defaults: new { controller = "Machines", action = "Index" }
                );

                routes.MapRoute(
                    name: "MachineRoute",
                    template: "Projects/{projectName}/{applicationName}/{clusterName}/{machineName}",
                    defaults: new { controller = "Machines", action = "Machine" }
                );

                routes.MapRoute(
                    name: "DeleteMachineRoute",
                    template: "Projects/{projectName}/{applicationName}/{clusterName}/{machineName}/Delete",
                    defaults: new { controller = "Machines", action = "DeleteMachine" }
                );

                routes.MapRoute(
                    name: "ClustersRoute",
                    template: "Projects/{projectName}/{applicationName}/Clusters",
                    defaults: new { controller = "Clusters", action = "Index" }
                );

                routes.MapRoute(
                    name: "ApplicationDownloadJsonRoute",
                    template: "Projects/{projectName}/{applicationName}/DownloadJson",
                    defaults: new { controller = "Projects", action = "DownloadJson" }
                );

                routes.MapRoute(
                    name: "ApplicationOpenJsonRoute",
                    template: "Projects/{projectName}/{applicationName}/OpenJson",
                    defaults: new { controller = "Projects", action = "OpenJson" }
                );

                routes.MapRoute(
                    name: "ApplicationCloneRoute",
                    template: "Projects/{projectName}/{applicationName}/Clone",
                    defaults: new { controller = "Projects", action = "Clone" }
                );

                routes.MapRoute(
                    name: "ProjectUpdateRoute",
                    template: "Projects/{projectName}/Update",
                    defaults: new { controller = "Projects", action = "Update" }
                );

                routes.MapRoute(
                     name: "ProjectRoute",
                     template: "Projects/{projectName}",
                     defaults: new { controller = "Projects", action = "Applications" }
                 );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }

    public class ClaimsTransformer : IClaimsTransformer
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = principal.Identity as ClaimsIdentity;

            if (identity.IsAuthenticated == false)
                return Task.FromResult<ClaimsPrincipal>(principal);

            var emailClaim = principal.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Email);

            if (emailClaim != null && !string.IsNullOrWhiteSpace(emailClaim.Value))
            {
                var adminUsers = ApplicationConfiguration.Get("super_admin_users").Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (adminUsers.Contains(emailClaim.Value))
                {
                    if (identity.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "superAdmin"))
                        return Task.FromResult<ClaimsPrincipal>(principal);

                    identity.AddClaim(new Claim(ClaimTypes.Role, "superAdmin"));
                }
            }

            var user = GetUser(identity);

            var access = JsonConvert.SerializeObject(user.Access, Formatting.Indented);

            identity.AddClaim(new Claim("SecurityAccess", access));

            return Task.FromResult<ClaimsPrincipal>(principal);
        }

        private User GetUser(ClaimsIdentity args)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            var claims = args.Claims;

            var userId = claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;

            string token = claims.Where(x => x.Type == "id_token").FirstOrDefault().Value;

            var url = hostName + "/api/Users/" + userId;

            var restClient = new RestSharp.RestClient(url);

            var request = new RestSharp.RestRequest();
            request.Method = RestSharp.Method.GET;
            request.AddHeader("Authorization", "Bearer " + token);

            var result = restClient.Execute(request);

            User user = null;

            if (string.IsNullOrWhiteSpace(result.Content) || result.Content.ToLowerInvariant() == "null")
            {
                user = new User();
                user.Id = userId;
                user.Access = new SecurityAccess();

                CreateUser(user, token);
            }
            else
            {
                user = JsonConvert.DeserializeObject<User>(result.Content);
            }

            return user;
        }

        private void CreateUser(User user, string token)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            var url = hostName + "/api/Users/" + user.Id;

            var restClient = new RestSharp.RestClient(url);

            var request = new RestSharp.RestRequest();
            request.Method = RestSharp.Method.POST;
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddHeader("Content-Type", "application/json;charset=utf-8");
            request.AddHeader("Authorization", "Bearer " + token);

            request.AddBody(user);

            restClient.Execute(request);
        }
    }
}
