using AnyStatus.API;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Plugins.Widgets.DevOps.CruiseControlNet
{
    /// <summary>
    /// Widget for projects on CruiseControl.Net integration servers.
    /// </summary>
    [DisplayName("CruiseControl.NET project")]
    [Description("CruiseControl.NET projects status.")]
    [DisplayColumn("DevOps")]
    public class CruiseControlNetWidget : Build, IHealthCheck, ISchedulable, IWebPage
    {
        private const string VIEW_FARM_REPORT = "ViewFarmReport.aspx";
        private const string XML_STATUS_REPORT = "XmlStatusReport.aspx";
        private const string CCNET_SEGMENT = "ccnet";

        /// <summary>
        /// Indicates the valid suffixes for an absolute URL to the CruiseControl.NET server.
        /// </summary>
        /// <remarks>
        /// Matches the end (last segment) of the following four URIs:
        ///   - http://CC_SERVER_NAME/ccnet/ViewFarmReport.aspx
        ///   - http://CC_SERVER_NAME/ccnet/XmlStatusReport.aspx
        ///   - http://CC_SERVER_NAME/ccnet/
        ///   - http://CC_SERVER_NAME/ccnet
        /// </remarks>
        private static readonly Regex ValidCruiseControlUrlPathSuffix =
            new Regex(@"(?:ViewFarmReport\.aspx|XmlStatusReport\.aspx|ccnet\/?)$",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        /// <summary>
        /// Stores the base URL used to build the final URL to either open the web browser or request the project status.
        /// </summary>
        private string _baseUrl;
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
            get { return this.ViewFarmReportUrl; }
            set { _baseUrl = this.ValidateUrl(value); }
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
        [Browsable(false)]
        public string ViewFarmReportUrl
          => string.IsNullOrWhiteSpace(this._baseUrl) ? string.Empty : _baseUrl + VIEW_FARM_REPORT;

        /// <summary>
        /// The URL where the CruiseControl.NET server published the XML status report of all projects.
        /// </summary>
        [Browsable(false)]
        public string XmlReportUrl
          => string.IsNullOrWhiteSpace(this._baseUrl) ? string.Empty : _baseUrl + XML_STATUS_REPORT;

        /// <summary>
        /// Verifies that the supplied <paramref name="url"/> is valid.
        /// </summary>
        /// <param name="url">The URL to verify if it points to a CruiseControl.Net server.</param>
        /// <returns></returns>
        /// <remarks>
        /// Valid URLs are http://CC_SERVER_NAME/ccnet/XmlStatusReport.aspx and http://CC_SERVER_NAME/ccnet/ViewFarmReport.aspx for example.
        /// </remarks>
        private string ValidateUrl(string url)
        {
            // Return an empty string if no useful string is supplied
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            try
            {
                // Parse the supplied string a valid URL or throw an exception if it is invalid
                var uri = new Uri(url, UriKind.Absolute);

                // Validate scheme is supported (i.e. http or https) 
                if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                {
                    return string.Empty;
                }

                // Remove query string from URL if present
                if (!string.IsNullOrWhiteSpace(uri.Query))
                {
                    uri = new Uri(uri.AbsoluteUri.Replace(uri.Query, string.Empty), UriKind.Absolute);
                }

                if (!ValidCruiseControlUrlPathSuffix.IsMatch(uri.AbsoluteUri))
                {
                    return string.Empty;
                }

                // Verify that the supplied URL consists of at least a single segment
                var segments = uri.Segments; // -> e.g. ["/", "ccnet/", "XmlStatusReport.aspx"]
                if (segments == null || segments.Length <= 0)
                {
                    return string.Empty;
                }

                // Check the last segment of the supplied URL
                var lastSegment = segments[segments.Length - 1];
                if (CCNET_SEGMENT.Equals(lastSegment, StringComparison.OrdinalIgnoreCase)
                    || $"{CCNET_SEGMENT}/".Equals(lastSegment, StringComparison.OrdinalIgnoreCase))
                {
                    // Return the base URL (e.g. http://CC_NET_SERVER/ccnet/) which will be used to open the browser and the web dashboard
                    return uri.AbsoluteUri.EndsWith("/") ? uri.AbsoluteUri : uri.AbsoluteUri + "/";
                }

                // Validate that the path either ends with web dashboard page 'ViewFarmReport.aspx' or XML report page 'XmlStatusReport.apsx'
                if (XML_STATUS_REPORT.Equals(lastSegment, StringComparison.OrdinalIgnoreCase)
                    || VIEW_FARM_REPORT.Equals(lastSegment, StringComparison.OrdinalIgnoreCase))
                {
                    // Return the base URL (e.g. http://CC_NET_SERVER/ccnet/) which will be used to open the browser and the web dashboard
                    var baseUrl = uri.AbsoluteUri.Replace(lastSegment, string.Empty);
                    return baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
                }

                return string.Empty;
            }
            catch (UriFormatException)
            {
                return string.Empty;
            }
        }
    }
}
