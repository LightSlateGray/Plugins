using AnyStatus.API;

namespace AnyStatus.Plugins.Widgets.DevOps.CruiseControlNet
{
    /// <summary>
    /// Opens the web page of the project in the user's web browser.
    /// </summary>
    public class CruiseControlOpenWebPage : OpenWebPage<CruiseControlNetWidget>
    {
        /// <summary>
        /// Create a new instance of the <see cref="CruiseControlOpenWebPage"/> class.
        /// </summary>
        /// <param name="processStarter">
        /// An implementation of the <see cref="IProcessStarter"/> interface supplied via
        /// dependency injection and passed through to the derived class.
        /// </param>
        public CruiseControlOpenWebPage(IProcessStarter processStarter) : base(processStarter) { }
    }
}
