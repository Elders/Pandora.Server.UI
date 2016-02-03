using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Elders.Pandora.Box;
using Newtonsoft.Json;

namespace Elders.Pandora.Server.UI.ViewModels
{
    public class Configuration
    {
        public string ProjectName { get; set; }

        public string ApplicationName { get; set; }

        public SecurityAccess SecurityAccess { get; set; }

        public List<ClusterDTO> Clusters { get; set; }

        public List<MachineDTO> Machines { get; set; }

        public DefaultsDTO Defaults { get; set; }

        public bool HasAccess()
        {
            if (SecurityAccess.Projects.Any(x => x.Name == this.ProjectName) &&
                SecurityAccess.Projects.SingleOrDefault(x => x.Name == this.ProjectName).Applications.Any(x => x.Name == this.ApplicationName))
                return true;

            else
                return false;
        }

        public bool HasAccess(string cluster)
        {
            if (this.HasAccess())
            {
                if (SecurityAccess
                    .Projects.SingleOrDefault(x => x.Name == this.ProjectName)
                    .Applications.SingleOrDefault(x => x.Name == this.ApplicationName)
                    .Clusters.Any(x => x.Name == cluster))
                    return true;

                else
                    return false;
            }
            else
                return false;
        }

        public class MachineDTO
        {
            public MachineDTO(string name, Cluster cluster, Dictionary<string, string> settings)
            {
                Name = name;
                Cluster = cluster;
                Settings = settings;
            }

            public string Name { get; set; }

            public Cluster Cluster { get; set; }

            public Dictionary<string, string> Settings { get; set; }

            public string this[string settingName]
            {
                get
                {
                    string value = String.Empty;
                    if (Settings.TryGetValue(settingName.ToLowerInvariant(), out value))
                    {
                        return value;
                    }
                    else
                    {
                        throw new System.Collections.Generic.KeyNotFoundException("SettingName does not exist in the collection");
                    }
                }
            }
        }

        public class ClusterDTO
        {
            public ClusterDTO(Cluster cluster, Dictionary<string, string> settings)
            {
                Cluster = cluster;
                Settings = settings;
            }

            public Cluster Cluster { get; set; }

            public Dictionary<string, string> Settings { get; set; }

            public string this[string settingName]
            {
                get
                {
                    string value = String.Empty;
                    if (Settings.TryGetValue(settingName.ToLowerInvariant(), out value))
                    {
                        return value;
                    }
                    else
                    {
                        throw new System.Collections.Generic.KeyNotFoundException("SettingName does not exist in the collection");
                    }
                }
            }
        }

        public class DefaultsDTO
        {
            public DefaultsDTO(Application application, Dictionary<string, string> settings)
            {
                Application = application;
                Settings = settings;
            }

            public Application Application { get; set; }

            public Dictionary<string, string> Settings { get; set; }

            public string this[string settingName]
            {
                get
                {
                    string value = String.Empty;
                    if (Settings.TryGetValue(settingName.ToLowerInvariant(), out value))
                    {
                        return value;
                    }
                    else
                    {
                        throw new System.Collections.Generic.KeyNotFoundException("SettingName does not exist in the collection");
                    }
                }
            }
        }
    }

    //public class Configuration
    //{
    //    private readonly Jar jar;

    //    private readonly ClaimsPrincipal user;

    //    public Configuration(ClaimsPrincipal user, Jar jar, string projectName)
    //    {
    //        this.jar = jar;
    //        this.user = user;
    //        SecurityAccess = GetSecurityAccess();
    //        ApplicationName = jar.Name;
    //        ProjectName = projectName;
    //    }

    //    public SecurityAccess SecurityAccess { get; set; }

    //    public string ProjectName { get; set; }

    //    public string ApplicationName { get; set; }

    //    public Dictionary<Cluster, Dictionary<string, string>> GetAllClusters()
    //    {
    //        if (!this.HasAccess())
    //            return new Dictionary<Cluster, Dictionary<string, string>>();

    //        var box = Box.Box.Mistranslate(jar);

    //        var pandora = new Pandora(box);

    //        var clusters = new Dictionary<Cluster, Dictionary<string, string>>();


    //        foreach (var env in SecurityAccess.Projects.SingleOrDefault(x => x.Name == this.ProjectName).Applications.SingleOrDefault(x => x.Name == ApplicationName).Clusters)
    //        {
    //            if (box.Clusters.Select(x => x.Name).Any(x => x == env.Name))
    //            {
    //                clusters.Add(env, pandora.Open(new PandoraOptions(env.Name, string.Empty, true)).AsDictionary());
    //            }
    //        }

    //        return clusters;
    //    }

    //    public KeyValuePair<Cluster, Dictionary<string, string>> GetCluster(string clusterName)
    //    {
    //        if (!this.HasAccess(clusterName))
    //            return new KeyValuePair<Cluster, Dictionary<string, string>>();

    //        var box = Box.Box.Mistranslate(jar);

    //        var pandora = new Pandora(box);

    //        return new KeyValuePair<Cluster, Dictionary<string, string>>(SecurityAccess.Projects
    //            .SingleOrDefault(x => x.Name == this.ProjectName)
    //            .Applications
    //            .SingleOrDefault(x => x.Name == this.ApplicationName)
    //            .Clusters
    //            .SingleOrDefault(x => x.Name == clusterName), pandora.Open(new PandoraOptions(clusterName, string.Empty, true)).AsDictionary());
    //    }

    //    public Dictionary<string, Dictionary<string, string>> GetAllMachines(string clusterName)
    //    {
    //        if (!this.HasAccess(clusterName))
    //            return new Dictionary<string, Dictionary<string, string>>();

    //        var box = Box.Box.Mistranslate(jar);

    //        var pandora = new Pandora(box);

    //        var machines = new Dictionary<string, Dictionary<string, string>>();

    //        foreach (var machine in box.Machines)
    //        {
    //            machines.Add(machine.Name, pandora.Open(new PandoraOptions(clusterName, machine.Name, true)).AsDictionary());
    //        }

    //        return machines;
    //    }

    //    public KeyValuePair<string, Dictionary<string, string>> GetMachine(string clusterName, string machineName)
    //    {
    //        if (!this.HasAccess(clusterName))
    //            return new KeyValuePair<string, Dictionary<string, string>>();

    //        var box = Box.Box.Mistranslate(jar);

    //        var pandora = new Pandora(box);

    //        return new KeyValuePair<string, Dictionary<string, string>>(machineName, pandora.Open(new PandoraOptions(clusterName, machineName, true)).AsDictionary());
    //    }

    //    public KeyValuePair<Application, Dictionary<string, string>> GetDefaults()
    //    {
    //        if (!this.HasAccess())
    //            return new KeyValuePair<Application, Dictionary<string, string>>();

    //        var app = SecurityAccess.Projects.SingleOrDefault(x => x.Name == this.ProjectName).Applications.SingleOrDefault(x => x.Name == this.ApplicationName);

    //        if (!app.Access.HasAccess(Access.ReadAcccess))
    //            return new KeyValuePair<Application, Dictionary<string, string>>();

    //        var box = Box.Box.Mistranslate(jar);

    //        return new KeyValuePair<Application, Dictionary<string, string>>(app, box.Defaults.AsDictionary());
    //    }

    //    private SecurityAccess GetSecurityAccess()
    //    {
    //        var securityAccessClaim = user.Claims.SingleOrDefault(x => x.Type == "SecurityAccess");

    //        SecurityAccess security;

    //        if (securityAccessClaim == null || string.IsNullOrWhiteSpace(securityAccessClaim.Value))
    //            security = new SecurityAccess();
    //        else
    //            security = JsonConvert.DeserializeObject<SecurityAccess>(securityAccessClaim.Value);

    //        if (security == null)
    //            security = new SecurityAccess();

    //        return security;
    //    }

    //    public bool HasAccess()
    //    {
    //        if (SecurityAccess.Projects.Any(x => x.Name == this.ProjectName) && SecurityAccess.Projects.SingleOrDefault(x => x.Name == this.ProjectName).Applications.Any(x => x.Name == this.ApplicationName))
    //            return true;

    //        else
    //            return false;
    //    }

    //    public bool HasAccess(string cluster)
    //    {
    //        if (this.HasAccess())
    //        {
    //            if (SecurityAccess
    //                .Projects.SingleOrDefault(x => x.Name == this.ProjectName)
    //                .Applications.SingleOrDefault(x => x.Name == this.ApplicationName)
    //                .Clusters.Any(x => x.Name == cluster))
    //                return true;

    //            else
    //                return false;
    //        }
    //        else
    //            return false;
    //    }
    //}
}
