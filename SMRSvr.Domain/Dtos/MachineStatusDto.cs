using SMRSvr.Domain.Enums;

namespace SMRSvr.Domain.Dtos;

public class MachineStatusDto
{
    /// <summary>
    /// 현재 동작 모드 (RUN, STOP, PAUSE 등)
    /// </summary>
    public string RunMode { get; set; } = string.Empty;

    /// <summary>
    /// 기계 좌표 (G53) - X, Y, Z, A, B
    /// </summary>
    public double[] Position { get; set; } = new double[5];

    /// <summary>
    /// 입력 접점 상태 (40개)
    /// </summary>
    public bool[] Inputs { get; set; } = new bool[40];

    /// <summary>
    /// 출력 접점 상태 (40개)
    /// </summary>
    public bool[] Outputs { get; set; } = new bool[40];

    /// <summary>
    /// 현재 장착된 툴 번호
    /// </summary>
    public int CurrentToolNo { get; set; }

    /// <summary>
    /// NC 파일 로드 여부
    /// </summary>
    public bool IsNcFileLoaded { get; set; }

    /// <summary>
    /// 로드된 NC 파일 이름
    /// </summary>
    public string NcFileName { get; set; } = string.Empty;

    /// <summary>
    /// NC 파일 전체 라인 수
    /// </summary>
    public uint TotalLines { get; set; }

    /// <summary>
    /// 현재 가공 진행 중인 라인 번호
    /// </summary>
    public uint CurrentLine { get; set; }

    /// <summary>
    /// 발생한 에러 코드 (0이면 정상)
    /// </summary>
    public int ErrorType { get; set; }
    public int ErrorCode { get; set; }

    /// <summary>
    /// 데이터 업데이트 시간
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
