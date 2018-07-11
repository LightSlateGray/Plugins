using System.Xml.Serialization;

namespace AnyStatus.Plugins.Widgets.DevOps.CruiseControlNet.Models
{
    /// <summary>
    /// Root element of the XML response from the CruiseControl.Net server.
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    [XmlRoot("Projects")]
    [XmlType(AnonymousType = true)]
    public class CruiseControlProjects
    {
        /// <summary>
        /// Contains the status of each integration project available on the CruiseControl.Net server.
        /// </summary>
        [XmlElement("Project")]
        public CruiseControlProjectStatus[] ProjectStatus { get; set; }

        /// <summary>
        /// The type of the CruiseControl server (should be <c>CCNet</c>).
        /// </summary>
        [XmlAttribute("CCType")]
        public string CruiseControlType { get; set; }
    }
}
