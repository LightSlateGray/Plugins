using AnyStatus.Plugins.Widgets.DevOps.CruiseControlNet.Enumerations;
using System;
using System.Xml.Serialization;

namespace AnyStatus.Plugins.Widgets.DevOps.CruiseControlNet.Models
{
    /// <summary>
    /// Value type that contains extensive details about a project's most recent integration.
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    [XmlRoot("Project")]
    public class CruiseControlProjectStatus
    {
        /// <summary>
        /// The name of the project.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// The category of the project.
        /// </summary>
        [XmlAttribute("category")]
        public string Category { get; set; }

        /// <summary>
        /// The current project activity.
        /// </summary>
        [XmlAttribute("activity")]
        public CruiseControlProjectActivity Activity { get; set; }

        /// <summary>
        /// The current integration status.
        /// </summary>
        [XmlAttribute("lastBuildStatus")]
        public CruiseControlIntegrationStatus BuildStatus { get; set; }

        /// <summary>
        /// The label of the last build.
        /// </summary>
        [XmlAttribute("lastBuildLabel")]
        public int BuildLabel { get; set; }

        /// <summary>
        /// The date and time the project last built successfully.
        /// </summary>
        [XmlAttribute("lastBuildTime")]
        public DateTime LastBuildTime { get; set; }

        /// <summary>
        /// The date and time the build will next be checked.
        /// </summary>
        [XmlAttribute("nextBuildTime")]
        public DateTime NextBuildTime { get; set; }

        /// <summary>
        /// The URL for viewing the project details.
        /// </summary>
        [XmlAttribute("webUrl")]
        public string WebURL { get; set; }

        /// <summary>
        /// The current stage of the build.
        /// </summary>
        [XmlAttribute("BuildStage")]
        public string BuildStage { get; set; }

        /// <summary>
        /// The name of the server this status is from.
        /// </summary>
        [XmlAttribute("serverName")]
        public string ServerName { get; set; }

        /// <summary>
        /// The description of the project (optional).
        /// </summary>
        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlArray("messages")]
        [XmlArrayItem("message")]
        public CruiseControlMessage[] Messages { get; set; }
    }
}
