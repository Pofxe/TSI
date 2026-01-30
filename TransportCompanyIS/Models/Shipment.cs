namespace TransportCompanyIS.Models;

public class Shipment
{
    public int Id { get; set; }
    public string FromAddress { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
