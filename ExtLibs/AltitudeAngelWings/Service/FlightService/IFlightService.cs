using System;
using AltitudeAngelWings.ApiClient.Models;

namespace AltitudeAngelWings.Service.FlightService
{
    public interface IFlightService : IDisposable
    {
        UserProfileInfo CurrentUser { get; }
    }
}