namespace SMRSvr.Domain.Dtos;

public class PositionDto
{
    public int CoordinateNo { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public double A { get; set; }
    public double B { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
