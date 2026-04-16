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
    /// 프로그램의 NC 파일 리스트에 새로운 NC 파일을 추가합니다.
    /// </summary>
    /// <param name="fullPath">추가할 NC 파일의 절대 경로</param>
    public bool AddNcFile(string fullPath)
    {
        // IPC_COMMAND_MANUAL = 99
        // IPC_SUBCMD_MAXXLINK_SERVICE = 31
        // Parameter format: "ANTL fullPath"
        return _controlService.SendCommand(
            EN_IPC_COMMAND.IPC_COMMAND_MANUAL, 
            31, 
            $"ANTL {fullPath}");
    }

    /// <summary>
    /// 가공할 NC 파일을 오픈합니다.
    /// </summary>
    public bool OpenNcFile(string fileName)
    {
        Console.WriteLine($"[NcFileService] Opening NC file: {fileName}");
        // TODO: MachineControlService를 통해 IPC_COMMAND_OPEN(5) 명령 전달
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
        return new List<string>();
    }
}
