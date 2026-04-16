using SMRSvr.Domain.Enums;

namespace SMRSvr.Domain.Dtos.MachineStatus;

public class MachineStatusDto
{
    public string RunMode { get; set; } = string.Empty;
    public double[] Position { get; set; } = new double[5];
    public bool[] Inputs { get; set; } = new bool[40];
    public bool[] Outputs { get; set; } = new bool[40];
    public int CurrentToolNo { get; set; }
    public bool IsNcFileLoaded { get; set; }
    public string NcFileName { get; set; } = string.Empty;
    public uint TotalLines { get; set; }
    public uint CurrentLine { get; set; }
    public int ErrorType { get; set; }
    public int ErrorCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
