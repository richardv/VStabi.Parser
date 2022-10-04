namespace VStabiParser.Interfaces
{
    using System.Threading.Tasks;

    public interface IVStabiReader
    {
        Task<string> Batteries(string controllerId);

        Task<string> Devices();

        Task<string> Flights(string controllerId, uint page = 0);

        Task<string> Screenshots(string controllerId, uint page = 0);

        Task<string> FlightDetail(string sid, int flightNo);

        Task<string> Models(string controllerId);

        Task<string> Setups();

        Task<string> Main();

        Task<string> SetupFile(string url);

        Task<string> DeviceSettings(string sid);

        Task<string> Battery(string name, string id);
    }
}