using SMRSvr.Domain.Entities;
using SMRSvr.Domain.Enums;

namespace SMRSvr.Infrastructure.Services;

/// <summary>
/// NC 파일 리스트 조회, 업로드, 삭제 및 Open/Close 명령 제어를 담당하는 서비스
/// </summary>
public class NcFileService
{
    private readonly SharedMemoryService _smService;
    private readonly MachineControlService _controlService;

    public NcFileService(SharedMemoryService smService, MachineControlService controlService)
    {
        _smService = smService;
        _controlService = controlService;
    }

    /// <summary>
    /// 가공할 NC 파일을 오픈합니다.
    /// </summary>
    public bool OpenNcFile(string fileName)
    {
        // TODO: MachineControlService를 통해 IPC_COMMAND_OPEN 명령 전달
        Console.WriteLine($"[NcFileService] Opening NC file: {fileName}");
        return true; 
    }

    /// <summary>
    /// 현재 오픈된 NC 파일을 닫습니다.
    /// </summary>
    public bool CloseNcFile()
    {
        Console.WriteLine("[NcFileService] Closing current NC file");
        return true;
    }

    /// <summary>
    /// 가공 경로 내의 NC 파일 리스트를 가져옵니다.
    /// </summary>
    public List<string> GetNcFileList()
    {
        // TODO: 실제 파일 시스템 또는 공유 메모리(NCFILE_MGR)에서 목록 조회
        return new List<string>();
    }
}
