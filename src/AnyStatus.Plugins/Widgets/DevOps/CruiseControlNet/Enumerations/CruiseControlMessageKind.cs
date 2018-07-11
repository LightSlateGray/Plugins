using System.Xml.Serialization;

namespace AnyStatus.Plugins.Widgets.DevOps.CruiseControlNet.Enumerations
{
    /// <summary>
    /// Enumeration that defines the kind of the message.
    /// </summary>
    public enum CruiseControlMessageKind
    {
        /// <summary>
        /// Undefined message kind.
        /// </summary>
        /// <remarks></remarks>
        [XmlEnum]
        NotDefined = 0,

        /// <summary>
        /// Indicates that the message contains the breakers of the integration.
        /// </summary>
        /// <remarks></remarks>
        [XmlEnum]
        Breakers = 1,

        /// <summary>
        /// Indicates that the message contains the fixers of the integration.
        /// </summary>
        /// <remarks></remarks>
        [XmlEnum]
        Fixer = 2,

        /// <summary>
        /// Indicates that the message contains the failing tasks of the integration.
        /// </summary>
        /// <remarks></remarks>
        [XmlEnum]
        FailingTasks = 3,

        /// <summary>
        /// Indicates that the message contains the status of the integration build.
        /// </summary>
        /// <remarks></remarks>
        [XmlEnum]
        BuildStatus = 4,

        /// <summary>
        /// Indicates that the message contains the user who aborted the integration.
        /// </summary>
        /// <remarks></remarks>
        [XmlEnum]
        BuildAbortedBy = 5
    }
}
