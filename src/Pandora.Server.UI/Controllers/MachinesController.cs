using System;
using System.Collections.Generic;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using Elders.Pandora.Server.UI.ViewModels;

namespace Elders.Pandora.Server.UI.Controllers
{
    [Authorize]
    public class MachinesController : Controller
    {
        public ActionResult Index(string projectName, string applicationName, string clusterName)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            var breadcrumbs = new List<KeyValuePair<string, string>>();
            breadcrumbs.Add(new KeyValuePair<string, string>("Projects", "/Projects"));
            breadcrumbs.Add(new KeyValuePair<string, string>(projectName, "/Projects/" + projectName));
            breadcrumbs.Add(new KeyValuePair<string, string>(applicationName, "/Projects/" + projectName + "/" + applicationName + "/Clusters"));
            ViewBag.Breadcrumbs = breadcrumbs;

            var config = GetConfig(projectName, applicationName);

            return View(new Tuple<string, Configuration>(clusterName, config));
        }

        [HttpPost]
        public ActionResult Index(string projectName, string applicationName, string clusterName, Dictionary<string, string> config)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            if (config.ContainsKey("controller"))
                return RedirectToAction("Index");

            var url = hostName + "/api/Clusters/" + projectName + "/" + applicationName + "/" + clusterName;

            var client = new RestSharp.RestClient(url);
            var request = new RestSharp.RestRequest(RestSharp.Method.PUT);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + User.IdToken());

            request.AddBody(JsonConvert.SerializeObject(config));

            var response = client.Execute(request);

            if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                throw response.ErrorException;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddMachine(string projectName, string applicationName, string clusterName, string machineName)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            var url = hostName + "/api/Machines/" + projectName + "/" + applicationName + "/" + machineName;

            var client = new RestSharp.RestClient(url);
            var request = new RestSharp.RestRequest(RestSharp.Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + User.IdToken());

            var jar = GetConfig(projectName, applicationName);

            request.AddBody(JsonConvert.SerializeObject(new Dictionary<string, string>()));

            var response = client.Execute(request);

            if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                throw response.ErrorException;
            }

            return RedirectToAction("Index");
        }

        public ActionResult Machine(string projectName, string applicationName, string clusterName, string machineName)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            var breadcrumbs = new List<KeyValuePair<string, string>>();
            breadcrumbs.Add(new KeyValuePair<string, string>("Projects", "/Projects"));
            breadcrumbs.Add(new KeyValuePair<string, string>(projectName, "/Projects/" + projectName));
            breadcrumbs.Add(new KeyValuePair<string, string>(applicationName, "/Projects/" + projectName + "/" + applicationName + "/Clusters"));
            breadcrumbs.Add(new KeyValuePair<string, string>(clusterName, "/Projects/" + projectName + "/" + applicationName + "/" + clusterName + "/Machines"));
            ViewBag.Breadcrumbs = breadcrumbs;

            var config = GetConfig(projectName, applicationName);

            return View(new Tuple<string, string, Configuration>(clusterName, machineName, config));
        }

        [HttpPost]
        public ActionResult Machine(string projectName, string applicationName, string clusterName, string machineName, Dictionary<string, string> config)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            if (config.ContainsKey("controller"))
                return RedirectToAction("Index");

            var url = hostName + "/api/Machines/" + projectName + "/" + applicationName + "/" + machineName;

            var client = new RestSharp.RestClient(url);
            var request = new RestSharp.RestRequest(RestSharp.Method.PUT);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + User.IdToken());

            request.AddBody(JsonConvert.SerializeObject(config));

            var response = client.Execute(request);

            if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                throw response.ErrorException;
            }

            return RedirectToAction("Machine");
        }

        private Configuration GetConfig(string projectName, string applicationName)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            var url = hostName + "/api/Configurations/" + projectName + "/" + applicationName;

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

            var config = JsonConvert.DeserializeObject<Configuration>(response.Content);

            return config;
        }
    }
}
