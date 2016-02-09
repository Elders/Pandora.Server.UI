using System;
using System.Collections.Generic;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using Elders.Pandora.Server.UI.ViewModels;
using System.Linq;

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

            var clusters = GetCluster(projectName, applicationName, clusterName);
            var machines = GetMachines(projectName, applicationName, clusterName);

            var config = new ConfigurationDTO()
            {
                ProjectName = projectName,
                ApplicationName = applicationName,
                Clusters = new List<ConfigurationDTO.ClusterDTO>() { new ConfigurationDTO.ClusterDTO(new ViewModels.Cluster(clusterName, Access.WriteAccess), clusters) },
                Machines = new List<ConfigurationDTO.MachineDTO>(machines.Select(x => new ConfigurationDTO.MachineDTO(x, new Cluster(clusterName, Access.WriteAccess), new Dictionary<string, string>())))
            };

            return View(new Tuple<string, ConfigurationDTO>(clusterName, config));
        }

        private List<string> GetMachines(string projectName, string applicationName, string clusterName)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            var url = hostName + "/api/Machines/ListMachines/" + projectName + "/" + applicationName + "/" + clusterName;

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

            var machines = JsonConvert.DeserializeObject<List<string>>(response.Content);

            return machines;
        }

        private Dictionary<string, string> GetMachine(string projectName, string applicationName, string clusterName, string machineName)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            var url = hostName + "/api/Machines/" + projectName + "/" + applicationName + "/" + clusterName + "/" + machineName;

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

            var machines = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

            return machines;
        }

        private Dictionary<string, string> GetCluster(string projectName, string applicationName, string clusterName)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            var url = hostName + "/api/Clusters/" + projectName + "/" + applicationName + "/" + clusterName;

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

            var cluster = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

            return cluster;
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

            var machine = GetMachine(projectName, applicationName, clusterName, machineName);

            var config = new ConfigurationDTO()
            {
                ProjectName = projectName,
                ApplicationName = applicationName,
                Clusters = new List<ConfigurationDTO.ClusterDTO>() { new ConfigurationDTO.ClusterDTO(new ViewModels.Cluster(clusterName, Access.WriteAccess), new Dictionary<string, string>()) },
                Machines = new List<ConfigurationDTO.MachineDTO>() { new ConfigurationDTO.MachineDTO(machineName, new Cluster(clusterName, Access.WriteAccess), machine) }
            };

            return View(new Tuple<string, string, ConfigurationDTO>(clusterName, machineName, config));
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

        public ActionResult DeleteMachine(string projectName, string applicationName, string clusterName, string machineName)
        {
            var hostName = ApplicationConfiguration.Get("pandora_api_url");
            var url = hostName + "/api/Machines/" + projectName + "/" + applicationName + "/" + machineName;

            var client = new RestSharp.RestClient(url);
            var request = new RestSharp.RestRequest(RestSharp.Method.DELETE);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + User.IdToken());
            var response = client.Execute(request);

            return RedirectToAction("Index", new { projectName = projectName, applicationName = applicationName, clusterName = clusterName });
        }

        private ConfigurationDTO GetConfig(string projectName, string applicationName)
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

            var config = JsonConvert.DeserializeObject<ConfigurationDTO>(response.Content);

            return config;
        }
    }
}
