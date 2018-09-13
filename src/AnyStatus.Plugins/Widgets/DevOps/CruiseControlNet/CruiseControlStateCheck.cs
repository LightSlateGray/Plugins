using AnyStatus.API;
using AnyStatus.Plugins.Widgets.DevOps.CruiseControlNet.Extensions;
using AnyStatus.Plugins.Widgets.DevOps.CruiseControlNet.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AnyStatus.Plugins.Widgets.DevOps.CruiseControlNet
{
    public class CruiseControlStateCheck : ICheckHealth<CruiseControlNetWidget>
    {
        private readonly ILogger _logger;

        public CruiseControlStateCheck(ILogger logger)
        {
            this._logger = Preconditions.CheckNotNull(logger, nameof(logger));
        }

        public async Task Handle(HealthCheckRequest<CruiseControlNetWidget> request, CancellationToken cancellationToken)
        {
            // Check for cancellation
            if (this.CheckForCancellation(request, cancellationToken))
            {
                // Update the current state to indicate an unknown state
                request.DataContext.State = State.Unknown;
                return;
            }

            // Retrieve the state of the requested project
            var projectStatus = await GetCruiseControlProjectStatusAsync(request.DataContext).ConfigureAwait(false);
            if (projectStatus == null)
            {
                this._logger.Info($"Could not retrieve current state of project '{request.DataContext.ProjectName}' from server '{request.DataContext.XmlStatusReportUrl}'.");
                request.DataContext.State = State.Invalid;
                return;
            }

            // Store retrieved data within properties for later user
            request.DataContext.ServerName = projectStatus.ServerName;
            request.DataContext.WebURL = projectStatus.WebURL;

            // Check for cancellation
            if (this.CheckForCancellation(request, cancellationToken))
            {
                // Update the current state to indicate an unknown state
                request.DataContext.State = State.Unknown;
                return;
            }

            var convertedState = projectStatus.BuildStatus.ToAnyStatusState();
            this._logger.Debug($"Retrieved status for project '{request.DataContext.ProjectName}': {projectStatus.BuildStatus} --> {convertedState}.");

            request.DataContext.State = convertedState;
        }

        private async Task<CruiseControlProjectStatus> GetCruiseControlProjectStatusAsync(CruiseControlNetWidget cruiseControlJob)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("ANYSTATUS", "1.0"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                HttpResponseMessage response = null;

                try
                {
                    // Request the XML status report from the CruiseControl.Net server and ensure that it was successful
                    response = await client.GetAsync(cruiseControlJob.XmlStatusReportUrl).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception exception)
                {
                    if (response == null)
                    {
                        this._logger.Error(exception, $"Did not receive any data in response of query for URL {cruiseControlJob.XmlStatusReportUrl}.");
                    }
                    else
                    {
                        this._logger.Error(exception, $"Received response code {response?.StatusCode}");
                    }
                    throw;
                }

                // Serialize the response and use the stream 
                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    XmlSerializer xmlSerializer = null;
                    try
                    {
                        xmlSerializer = new XmlSerializer(typeof(CruiseControlProjects));
#if DEBUG
                        xmlSerializer.UnknownAttribute += XmlSerializer_UnknownAttribute;
                        xmlSerializer.UnknownElement += XmlSerializer_UnknownElement;
                        xmlSerializer.UnknownNode += XmlSerializer_UnknownNode;
                        xmlSerializer.UnreferencedObject += XmlSerializer_UnreferencedObject;
#endif
                        var cruiseControlProjects = (CruiseControlProjects)xmlSerializer.Deserialize(stream);
                        if (cruiseControlProjects?.ProjectStatus != null)
                        {
                            // Find the matching project status based on the project's name
                            var projectStatus = Array.Find(
                                cruiseControlProjects.ProjectStatus,
                                project => cruiseControlJob.ProjectName.Equals(project.Name, StringComparison.OrdinalIgnoreCase));

                            if (projectStatus == null)
                            {
                                var validNames = string.Join(",", cruiseControlProjects.ProjectStatus.Select(project => project.Name));
                                this._logger.Debug($"Could not find the matching status for project {cruiseControlJob.ProjectName}. Valid project names are: {validNames}.");
                            }

                            return projectStatus;
                        }
                        else
                        {
                            this._logger.Info($"Could not parse any project status information from the response.");
                        }
                    }
                    catch (Exception exception)
                    {
                        this._logger.Error(exception, $"Exception occurred during de-serialization: {exception.Message}");
                    }
#if DEBUG
                    finally
                    {
                        if (xmlSerializer != null)
                        {
                            xmlSerializer.UnknownAttribute -= XmlSerializer_UnknownAttribute;
                            xmlSerializer.UnknownElement -= XmlSerializer_UnknownElement;
                            xmlSerializer.UnknownNode -= XmlSerializer_UnknownNode;
                            xmlSerializer.UnreferencedObject -= XmlSerializer_UnreferencedObject;
                        }
                    }
#endif
                }

                return null;
            }
        }

        /// <summary>
        /// Determines whether cancellation has been requested by the supplied <paramref name="cancellationToken"/>.
        /// </summary>
        /// <param name="request">The health check request for a CruiseControl.Net project.</param>
        /// <param name="cancellationToken">The cancellation token with which cancellation can be requested.</param>
        /// <returns><c>true</c> if cancellation has been requested; otherwise <c>false</c>.</returns>
        private bool CheckForCancellation(HealthCheckRequest<CruiseControlNetWidget> request, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            this._logger.Debug("Health check canceled due to request by cancellation token.");
            request.DataContext.State = State.Unknown;

            return true;
        }

#if DEBUG
        private void XmlSerializer_UnknownAttribute(object sender, XmlAttributeEventArgs eventArgs)
        {
            this._logger.Info($"Found unknown attribute '{eventArgs.Attr.Name}' at line {eventArgs.LineNumber}, position {eventArgs.LinePosition}: '{eventArgs.Attr}' during de-serialization of type {eventArgs.ObjectBeingDeserialized?.GetType().FullName}.");
        }

        private void XmlSerializer_UnknownElement(object sender, XmlElementEventArgs eventArgs)
        {
            this._logger.Info($"Found unknown element '{eventArgs.Element.Name}' at line {eventArgs.LineNumber}, position {eventArgs.LinePosition}: '{eventArgs.Element}' during de-serialization of type {eventArgs.ObjectBeingDeserialized?.GetType().FullName}.");
        }

        private void XmlSerializer_UnknownNode(object sender, XmlNodeEventArgs eventArgs)
        {
            this._logger.Info($"Found unknown node type {eventArgs.NodeType} called '{eventArgs.Name}' at line {eventArgs.LineNumber}, position {eventArgs.LinePosition}: '{eventArgs.NodeType}' during de-serialization of type {eventArgs.ObjectBeingDeserialized?.GetType().FullName}.");
        }

        private void XmlSerializer_UnreferencedObject(object sender, UnreferencedObjectEventArgs eventArgs)
        {
            this._logger.Info($"Found unreferenced object with identifier {eventArgs.UnreferencedId}: {eventArgs.UnreferencedObject}.");
        }
#endif
    }
}
