using System.Xml.Serialization;

namespace AnyStatus.Plugins.Widgets.DevOps.CruiseControlNet.Enumerations
{
    /// <summary>
    /// Enumeration of the possible activities of a project under continuous integration by CruiseControl.NET.
    /// </summary>
    public enum CruiseControlProjectActivity
    {
        /// <summary>
        /// A default state indicating an unknown project activity.
        /// </summary>
        [XmlEnum]
        NotDefined = 0,

        /// <summary>
        /// CruiseControl.NET is checking for modifications in this project's source control system.
        /// </summary>
        [XmlEnum]
        CheckingModifications = 1,

        /// <summary>
        /// CruiseControl.NET is running the build phase of the project's integration.
        /// </summary>
        [XmlEnum]
        Building = 2,

        /// <summary>
        /// CruiseControl.NET is sleeping, and no activity is being performed for this project.
        /// </summary>
        [XmlEnum]
        Sleeping = 3,

        /// <summary>
        /// CruiseControl.NET is queuing a pending build integration request for this project.
        /// </summary>
        [XmlEnum]
        Pending = 4,
    }
}
