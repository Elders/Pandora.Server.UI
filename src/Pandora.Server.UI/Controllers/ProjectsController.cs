using System;
using System.Collections.Generic;
using Elders.Pandora.Box;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using Elders.Pandora.Server.UI.Common;
using Elders.Pandora.Server.UI.ViewModels;
using RestSharp.Extensions.MonoHttp;

namespace Elders.Pandora.Server.UI.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        public ActionResult Index()
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            var url = hostName + "/api/Projects";

            var client = new RestSharp.RestClient(url);
            var request = new RestSharp.RestRequest(RestSharp.Method.GET);
            request.AddHeader("Authorization", "Bearer " + User.IdToken());
            var response = client.Execute(request);

            if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                throw response.ErrorException;
            }

            var projects = JsonConvert.DeserializeObject<List<string>>(response.Content);

            return View(projects);
        }

        [HttpPost]
        public ActionResult Index(string projectName, string gitUrl)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            var url = hostName + "/api/Projects/" + projectName;

            var client = new RestSharp.RestClient(url);
            var request = new RestSharp.RestRequest(RestSharp.Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + User.IdToken());
            request.AddBody(gitUrl);

            var response = client.Execute(request);

            return Redirect("/Projects/" + projectName);
        }

        public ActionResult Applications(string projectName)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            var breadcrumbs = new List<KeyValuePair<string, string>>();
            breadcrumbs.Add(new KeyValuePair<string, string>("Projects", "/Projects"));
            ViewBag.Breadcrumbs = breadcrumbs;

            var url = hostName + "/api/Jars/ListJars/" + projectName;

            var client = new RestSharp.RestClient(url);
            var request = new RestSharp.RestRequest(RestSharp.Method.GET);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + User.IdToken());
            var response = client.Execute(request);

            if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                throw response.ErrorException;
            }

            var configsNames = JsonConvert.DeserializeObject<List<string>>(response.Content);

            var configs = new List<ConfigurationDTO>();

            foreach (var config in configsNames)
            {
                configs.Add(new ConfigurationDTO() { ProjectName = projectName, ApplicationName = config });
            }

            return View(configs);
        }

        [HttpPost]
        public ActionResult Applications(string projectName, string applicationName, string fileName, string config)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            var url = hostName + "/api/Jars/" + projectName + "/" + applicationName;

            var client = new RestSharp.RestClient(url);
            var request = new RestSharp.RestRequest(RestSharp.Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + User.IdToken());

            if (!string.IsNullOrWhiteSpace(config))
            {
                try
                {
                    var jar = JsonConvert.DeserializeObject<Jar>(config);
                    var box = Box.Box.Mistranslate(jar);
                }
                catch (Exception)
                {
                    var jar = new Jar();
                    jar.Name = applicationName;

                    config = JsonConvert.SerializeObject(jar);
                }
            }
            else
            {
                var jar = new Jar();
                jar.Name = applicationName;

                config = JsonConvert.SerializeObject(jar);
            }

            request.AddBody(config);

            var response = client.Execute(request);

            Elders.Pandora.Server.UI.ViewModels.User.GiveAccess(User, projectName, applicationName, "Defaults", Access.WriteAccess);

            return Redirect("/Projects/" + projectName + "/" + applicationName + "/Clusters");
        }

        public ActionResult Update(string projectName)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            var url = hostName + "/api/Projects/Update/" + projectName;

            var client = new RestSharp.RestClient(url);
            var request = new RestSharp.RestRequest(RestSharp.Method.PUT);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + User.IdToken());

            var response = client.Execute(request);

            if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                throw response.ErrorException;
            }

            return RedirectToAction("Index");
        }

        public ActionResult OpenJson(string projectName, string applicationName)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");

            var url = hostName + "/api/Jars/" + projectName + "/" + applicationName;

            var client = new RestSharp.RestClient(url);
            var request = new RestSharp.RestRequest(RestSharp.Method.GET);
            request.AddHeader("Authorization", "Bearer " + User.IdToken());

            var response = client.Execute(request);

            var config = JsonConvert.DeserializeObject<dynamic>(response.Content);

            return View("_ApplicationJsonView", model: JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        public FileResult DownloadJson(string projectName, string applicationName)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");

            var url = hostName + "/api/Jars/" + projectName + "/" + applicationName;

            var client = new RestSharp.RestClient(url);
            var request = new RestSharp.RestRequest(RestSharp.Method.GET);
            request.AddHeader("Authorization", "Bearer " + User.IdToken());

            var response = client.Execute(request);

            string fileName = applicationName + ".json";

            byte[] bytes = new byte[response.Content.Length * sizeof(char)];
            Buffer.BlockCopy(response.Content.ToCharArray(), 0, bytes, 0, bytes.Length);

            return File(bytes, MimeMapping.GetMimeMapping(fileName), fileName);
        }
    }
}
