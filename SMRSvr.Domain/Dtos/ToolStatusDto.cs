namespace SMRSvr.Domain.Dtos;

public class ToolStatusDto
{
    public int ToolNo { get; set; }
    public double UsingRate { get; set; }
    public int ErrorCode { get; set; } // 변환된 에러 코드 (1~6)
    public string ErrorMessage { get; set; } = "None";
    public uint UsingTime { get; set; }
    public uint MaximumTime { get; set; }
}

public class ToolListDto
{
    public int TotalToolCount { get; set; }
    public List<ToolStatusDto> Tools { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
