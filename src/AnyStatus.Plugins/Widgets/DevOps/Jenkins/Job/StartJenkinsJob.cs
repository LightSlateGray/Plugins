﻿using AnyStatus.API;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class StartJenkinsJob : IStart<JenkinsJob_v1>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public StartJenkinsJob(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task Handle(StartRequest<JenkinsJob_v1> request, CancellationToken cancellationToken)
        {
            var dialog = new ConfirmationDialog($"Are you sure you want to start {request.DataContext.Name}?");

            var result = _dialogService.ShowDialog(dialog);

            if (result != DialogResult.Yes)
                return;

            var jenkinsClient = new JenkinsClient(_logger);

            await jenkinsClient.TriggerJobAsync(request.DataContext).ConfigureAwait(false);

            request.DataContext.State = State.Queued;

            _logger.Info($"\"Jenkins job {request.DataContext.Name}\" has been queued.");
        }
    }
}