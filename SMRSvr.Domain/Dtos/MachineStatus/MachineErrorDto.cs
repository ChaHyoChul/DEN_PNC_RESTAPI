namespace SMRSvr.Domain.Dtos.MachineStatus;

public class MachineErrorDto
{
    public int ErrorType { get; set; }
    public int ErrorCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
