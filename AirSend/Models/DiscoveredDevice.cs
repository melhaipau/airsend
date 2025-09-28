
namespace AirSend.Models
{
    public class DiscoveredDevice
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Hostname { get; set; } = string.Empty;
        public string IP { get; set; } = string.Empty;
        public int Port { get; set; }
        public string ServiceType { get; set; } = string.Empty; // _airplay._tcp or _raop._tcp
        public bool IsOnline { get; set; } = true;
    }
}
