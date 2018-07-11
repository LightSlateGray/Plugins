using AnyStatus.Plugins.Widgets.DevOps.CruiseControlNet.Enumerations;
using System.Xml.Serialization;

namespace AnyStatus.Plugins.Widgets.DevOps.CruiseControlNet.Models
{
    /// <summary>
    /// A user-readable message.
    /// </summary>
    public class CruiseControlMessage
    {
        /// <summary>
        /// The text of the message.
        /// </summary>
        [XmlText]
        [XmlAttribute("text")]
        public string Text { get; set; }

        /// <summary>
        /// The type of message.
        /// </summary>
        [XmlAttribute("kind")]
        public CruiseControlMessageKind Kind { get; set; }
    }
}
