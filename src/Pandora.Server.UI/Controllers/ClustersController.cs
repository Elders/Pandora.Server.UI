using System.Collections.Generic;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using Elders.Pandora.Server.UI.ViewModels;
using Elders.Pandora.Box;
using System.Linq;

namespace Elders.Pandora.Server.UI.Controllers
{
    [Authorize]
    public class ClustersController : Controller
    {
        public ActionResult Index(string projectName, string applicationName)
        {
            var breadcrumbs = new List<KeyValuePair<string, string>>();
            breadcrumbs.Add(new KeyValuePair<string, string>("Projects", "/Projects"));
            breadcrumbs.Add(new KeyValuePair<string, string>(projectName, "/Projects/" + projectName));
            ViewBag.Breadcrumbs = breadcrumbs;

            var defaults = GetDefaultSettings(projectName, applicationName);
            var clusterNames = GetClusters(projectName, applicationName);

            var config = new ConfigurationDTO()
            {
                ProjectName = projectName,
                ApplicationName = applicationName,
                Defaults = new ConfigurationDTO.DefaultsDTO(new Application() { Access = Access.WriteAccess }, defaults)
            };

            if (ReferenceEquals(null, clusterNames) == false)
            {
                config.Clusters = new List<ConfigurationDTO.ClusterDTO>(clusterNames.Select(x => new ConfigurationDTO.ClusterDTO(new ViewModels.Cluster(x, Access.WriteAccess), new ConfigurationDTO.Settings())));
            }

            return View(config);
        }

        private IEnumerable<string> GetClusters(string projectName, string applicationName)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");

            var url = hostName + "/api/Clusters/ListClusters/" + projectName + "/" + applicationName;

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

            var config = JsonConvert.DeserializeObject<List<string>>(response.Content);

            return config;
        }

        private ConfigurationDTO.Settings GetDefaultSettings(string projectName, string applicationName)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");

            var url = hostName + "/api/Defaults/" + projectName + "/" + applicationName;

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

            var config = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content)
                .Select(x => new ConfigurationDTO.SettingDTO(x.Key, x.Value));

            return new ConfigurationDTO.Settings(config);
        }

        [HttpPost]
        public ActionResult Index(string projectName, string applicationName, string clusterName)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            var newCluster = new Elders.Pandora.Box.Cluster(clusterName, new Dictionary<string, string>());

            var url = hostName + "/api/Clusters/" + projectName + "/" + applicationName + "/" + clusterName;

            var client = new RestSharp.RestClient(url);
            var request = new RestSharp.RestRequest(RestSharp.Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + User.IdToken());
            request.AddBody(JsonConvert.SerializeObject(new Dictionary<string, string>()));

            var response = client.Execute(request);

            if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                throw response.ErrorException;
            }

            ViewModels.User.GiveAccess(User, projectName, applicationName, clusterName, Access.WriteAccess);

            var defaultSettings = GetDefaultSettings(projectName, applicationName);
            var clusterNames = GetClusters(projectName, applicationName);

            var config = new ConfigurationDTO()
            {
                ProjectName = projectName,
                ApplicationName = applicationName,
                Defaults = new ConfigurationDTO.DefaultsDTO(new Application() { Access = Access.WriteAccess }, defaultSettings),
                Clusters = new List<ConfigurationDTO.ClusterDTO>(clusterNames.Select(x => new ConfigurationDTO.ClusterDTO(new ViewModels.Cluster(x, Access.WriteAccess), new ConfigurationDTO.Settings())))
            };

            return View(config);
        }

        [HttpPost]
        public ActionResult Defaults(string projectName, string applicationName, ConfigurationDTO.Settings config)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            if (config.Any(x => x.Key == "controller"))
                return RedirectToAction("Index");

            var url = hostName + "/api/Defaults/" + projectName + "/" + applicationName;

            var client = new RestSharp.RestClient(url);
            var request = new RestSharp.RestRequest(RestSharp.Method.PUT);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + User.IdToken());
            var body = config.ToDictionary(x => x.Key, x => x.Value);
            request.AddBody(body);

            var response = client.Execute(request);

            if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                throw response.ErrorException;
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult GetSettingRow(string key, string value, int index)
        {
            var setting = new ConfigurationDTO.SettingDTO(key, value);

            ViewData["index"] = index;

            return PartialView("EditorTemplates/SettingEditor", setting);
        }
    }
}
