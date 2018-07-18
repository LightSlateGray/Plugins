using AnyStatus.Plugins.Widgets.DevOps.CruiseControlNet.Enumerations;
using AnyStatus.API;

namespace AnyStatus.Plugins.Widgets.DevOps.CruiseControlNet.Extensions
{
    /// <summary>
    /// Extension methods for enumeration <see cref="CruiseControlIntegrationStatus"/>.
    /// </summary>
    internal static class CruiseControlIntegrationStatusExtensions
    {
        /// <summary>
        /// Converts the <see cref="CruiseControlIntegrationStatus"/> value to the corresponding AnyStatus <see cref="State"/>.
        /// </summary>
        /// <param name="cruiseControlIntegrationStatus">
        /// The enumeration value to convert to the corresponding AnyStatus <see cref="State"/>.
        /// </param>
        /// <returns></returns>
        internal static State ToAnyStatusState(this CruiseControlIntegrationStatus cruiseControlIntegrationStatus)
        {
            switch (cruiseControlIntegrationStatus)
            {
                case CruiseControlIntegrationStatus.NotDefined:
                case CruiseControlIntegrationStatus.Unknown:
                    return State.Invalid;
                case CruiseControlIntegrationStatus.Success:
                    return State.Ok;
                case CruiseControlIntegrationStatus.Failure:
                    return State.Failed;
                case CruiseControlIntegrationStatus.Exception:
                    return State.Error;
                case CruiseControlIntegrationStatus.Cancelled:
                    return State.Canceled;
                default:
                    return State.Invalid;
            }
        }
    }
}
