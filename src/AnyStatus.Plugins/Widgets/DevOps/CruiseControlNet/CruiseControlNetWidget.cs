using AnyStatus.API;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Plugins.Widgets.DevOps.CruiseControlNet
{
    /// <summary>
    /// Widget for projects on CruiseControl.Net integration servers.
    /// </summary>
    [DisplayName("CruiseControl.NET project")]
    [Description("CruiseControl.NET projects status.")]
    [DisplayColumn("DevOps")]
    public class CruiseControlNetWidget : Build, IHealthCheck, ISchedulable, IStartable, IWebPage
    {
        /// <summary>
        /// Constant value referring to the web dashboard of the CruiseControl.NET farm overview.
        /// </summary>
        private const string VIEW_FARM_REPORT = "ViewFarmReport.aspx";

        /// <summary>
        /// Constant value referring to the web page where the CruiseControl.NET Server publishes the current status
        /// of all integration projects as XML.
        /// </summary>
        private const string XML_STATUS_REPORT = "XmlStatusReport.aspx";

        /// <summary>
        /// Stores the URL supplied by the user which is used to build the final URL to either open the web browser
        /// or request the project status as XML.
        /// </summary>
        private string _url;

        /// <summary>
        /// Stores the name of the project for which the status is requested.
        /// </summary>
        private string _projectName;

        /// <summary>
        /// The URL used for the "Open in Browser" command.
        /// </summary>
        [Url]
        [Required]
        [PropertyOrder(10)] //sets the order in which the property will appear in the category. Smaller is higher.
        [Category("CruiseControl.NET")]
        [Description(
            "CruiseControl.NET web dashboard URL. " +
            "Usually, this refers to http://CC_SERVER_NAME/ccnet/ViewFarmReport.aspx or something similar.")]
        public string URL
        {
            get { return _url; }
            set
            {
                _url = value;

                // Get base URL from the supplied value and update the URls used internally
                var baseUrl = GetBaseUrl(value);
                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    this.ViewFarmReportUrl = string.Empty;
                    this.XmlStatusReportUrl = string.Empty;
                }
                else
                {
                    this.ViewFarmReportUrl = baseUrl + VIEW_FARM_REPORT;
                    this.XmlStatusReportUrl = baseUrl + XML_STATUS_REPORT;
                }
            }
        }

        /// <summary>
        /// The name of the project to check its state.
        /// </summary>
        [Required]
        [PropertyOrder(20)]
        [Category("CruiseControl.NET")]
        [Description("CruiseControl.NET project name as seen in the web dashboard.")]
        public string ProjectName
        {
            get { return _projectName; }
            set { _projectName = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim(); }
        }

        /// <summary>
        /// The URL of the CruiseControl.NET server web dashboard, also known as farm report.
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string ViewFarmReportUrl { get; private set; }

        /// <summary>
        /// The URL where the CruiseControl.NET server publishes the XML status report of all projects.
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string XmlStatusReportUrl { get; private set; }

        /// <summary>
        /// Retrieves the base URL from the supplied <paramref name="url"/> which is used to construct the URL of the
        /// web dashboard (<see cref="ViewFarmReportUrl"/>) and the URL from which the XML response containing the status
        /// of all integration projects will be retrieved (<see cref="XmlStatusReportUrl"/>).
        /// </summary>
        /// <param name="url">The URL to verify if it points to a CruiseControl.Net server.</param>
        /// <returns></returns>
        /// <remarks>
        /// Valid URLs include http://CC_SERVER_NAME/ccnet/XmlStatusReport.aspx, http://CC_SERVER_NAME/ccnet/ViewFarmReport.aspx
        /// and http://CC_SERVER_NAME/ccnet/local/ViewStatusReport.aspx for example.
        /// </remarks>
        private static string GetBaseUrl(string url)
        {
            try
            {
                // Parse the supplied string as valid URL or throw an exception if it is invalid
                var uri = new Uri(url, UriKind.Absolute);

                // Remove query string from URL if present
                if (!string.IsNullOrWhiteSpace(uri.Query))
                {
                    uri = new Uri(uri.AbsoluteUri.Replace(uri.Query, string.Empty), UriKind.Absolute);
                }

                // Check if the URL ends with a slash
                if (uri.AbsoluteUri.EndsWith("/"))
                {
                    return uri.AbsoluteUri;
                }
                else
                {
                    return uri.AbsoluteUri.Substring(0, uri.AbsoluteUri.LastIndexOf('/') + 1);
                }
            }
            catch (UriFormatException)
            {
                return string.Empty;
            }
        }
    }
}
