using AnyStatus.API;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Widgets.DevOps.CruiseControlNet
{
    public class CruiseControlStartBuildJob : IStart<CruiseControlNetWidget>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public CruiseControlStartBuildJob(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task Handle(StartRequest<CruiseControlNetWidget> request, CancellationToken cancellationToken)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want force a new integration build for project {request.DataContext.ProjectName}?");

            var result = _dialogService.ShowDialog(dialog);
            if (result != DialogResult.Yes)
            {
                return;
            }

            await this.PostForceCruiseControlProjectBuildAsync(request.DataContext).ConfigureAwait(false);

            _logger.Info($"Build request for project {request.DataContext.ProjectName} aka. \"{request.DataContext.Name}\" has been triggered.");
        }

        private async Task<bool> PostForceCruiseControlProjectBuildAsync(CruiseControlNetWidget cruiseControlJob)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("ANYSTATUS", "1.0"));

                try
                {
                    // Build content for URL encoded HTTP post request
                    var postContent = new FormUrlEncodedContent(
                        new[]
                        {
                            new KeyValuePair<string, string>("forceBuild", "true"),
                            new KeyValuePair<string, string>("projectName", cruiseControlJob.ProjectName),
                            new KeyValuePair<string, string>("serverName", "local"),
                        });

                    // Post a request to force a new integration build the project
                    var response = await client.PostAsync(cruiseControlJob.ViewFarmReportUrl, postContent).ConfigureAwait(false);

                    // Ensure that the request has been handled successfully
                    response.EnsureSuccessStatusCode();

                    return true;
                }
                catch (Exception exception)
                {
                    this._logger.Error(exception, $"Something went wrong during force build request for project {cruiseControlJob.ProjectName} on URL {cruiseControlJob.ViewFarmReportUrl}.");
                    throw;
                }
            }
        }
    }
}
