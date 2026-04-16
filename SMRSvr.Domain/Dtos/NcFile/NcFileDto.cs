namespace SMRSvr.Domain.Dtos.NcFile;

public class NcFileDto
{
    public string ID { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public uint FileSize { get; set; }
    public string State { get; set; } = "BEFORE";
    public bool IsFinished { get; set; }
    public bool IsSelected { get; set; }
}

public class NcFileListDto
{
    public int TotalCount { get; set; }
    public List<NcFileDto> Files { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
