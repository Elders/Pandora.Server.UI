using System.Collections.Generic;
using System.Linq;

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
            if (SecurityAccess.Projects.Any(x => x.Name == ProjectName) &&
                SecurityAccess.Projects.SingleOrDefault(x => x.Name == ProjectName).Applications.Any(x => x.Name == ApplicationName))
                return true;

            else
                return false;
        }

        public bool HasAccess(string cluster)
        {
            if (HasAccess())
            {
                return SecurityAccess
                    .Projects.SingleOrDefault(x => x.Name == this.ProjectName)
                    .Applications.SingleOrDefault(x => x.Name == this.ApplicationName)
                    .Clusters
                    .Any(x => x.Name == cluster);
            }

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

            public string this[string settingKey]
            {
                get
                {
                    return Settings[settingKey.ToLowerInvariant()];
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

            public string this[string settingKey]
            {
                get
                {
                    return Settings[settingKey.ToLowerInvariant()];
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

            public string this[string settingKey]
            {
                get
                {
                    return Settings[settingKey.ToLowerInvariant()];
                }
            }
        }
    }
}
