using System.Xml.Serialization;

namespace AnyStatus.Plugins.Widgets.DevOps.CruiseControlNet.Enumerations
{
    /// <summary>
    /// Enumeration of possible summations following the integration of a project.
    /// </summary>
    public enum CruiseControlIntegrationStatus
    {
        /// <summary>
        /// A default state indicating an unknown or undefined integration status.
        /// </summary>
        [XmlEnum]
        NotDefined = 0,

        /// <summary>
        /// Indicates the project's integration was successful.
        /// Compilation succeeded, and any tests passed.
        /// </summary>
        [XmlEnum]
        Success = 1,

        /// <summary>
        /// Indicates the project's integration failed.
        /// Either the compilation or tests failed.
        /// </summary>
        [XmlEnum]
        Failure = 2,

        /// <summary>
        /// Indicated CruiseControl.NET experienced exceptional circumstances during the
        /// integration of the project.
        /// </summary>
        [XmlEnum]
        Exception = 3,

        /// <summary>
        /// Indicates the state of the most recent integration is unknown.
        /// Perhaps no integration has yet occurred.
        /// </summary>
        [XmlEnum]
        Unknown = 4,

        /// <summary>
        /// Indicates that the most recent integration was canceled.
        /// </summary>
        [XmlEnum]
        Cancelled = 5,
    }
}
