using FlightSightings.Models;

namespace FlightSightings.Interfaces
{
    public interface IFlight
    {
        List<Flight> GetItems(string SortProperty, SortOrder sortOrder, string SearchText = "");

        Flight Details(int id);

        Flight Edit(int id);

        Task<Flight> Edit(Flight flight);

        Flight Delete(int id);

        Flight Delete(Flight flight);

        Task<Flight> Create(Flight flight);
    }
}
