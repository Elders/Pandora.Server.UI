using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Elders.Pandora.Box;
using Newtonsoft.Json;

namespace Elders.Pandora.Server.UI.ViewModels
{
    public class ConfigurationDTO
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
}
