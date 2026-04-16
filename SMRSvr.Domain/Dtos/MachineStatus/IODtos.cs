namespace SMRSvr.Domain.Dtos.MachineStatus;

public class IODtos
{
    public bool[] Inputs { get; set; } = new bool[40];
    public bool[] Outputs { get; set; } = new bool[40];
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
