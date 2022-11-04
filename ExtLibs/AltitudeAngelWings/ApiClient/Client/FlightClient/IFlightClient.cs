using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AltitudeAngelWings.ApiClient.Client.FlightClient
{
    public interface IFlightClient : IDisposable
    {
        Task<JObject> StartFlight(string notificationProtocolUrl, string flightPlanId, string deconflictionService);
        Task CompleteFlight(string flightId);
        Task AcceptInstruction(string instructionId);
        Task RejectInstruction(string instructionId);
    }
}