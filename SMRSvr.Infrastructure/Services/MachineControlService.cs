using SMRSvr.Domain.Entities;
using SMRSvr.Domain.Enums;

namespace SMRSvr.Infrastructure.Services;

/// <summary>
/// 장비의 동작 제어(Run, Stop, Pause, EMG 등)를 담당하는 서비스
/// </summary>
public class MachineControlService
{
    private readonly SharedMemoryService _smService;
    private const string QueueName = "IPC_SERVER";

    public MachineControlService(SharedMemoryService smService)
    {
        _smService = smService;
    }

    /// <summary>
    /// IPC 명령을 큐에 추가합니다.
    /// </summary>
    public unsafe bool SendCommand(EN_IPC_COMMAND mainCmd, byte subCmd = 0, string param = "")
    {
        var command = new SIpcCommCommand
        {
            m_cmd = (byte)mainCmd
        };

        if (subCmd > 0 || !string.IsNullOrEmpty(param))
        {
            byte* p = command.param;
            p[0] = subCmd; 
            if (!string.IsNullOrEmpty(param))
            {
                SharedMemoryService.SetAnsiString(p + 1, param, 62);
            }
        }
        
        return _smService.EnqueueCommand(QueueName, command);
    }

    /// <summary>
    /// 정수형 파라미터를 포함한 IPC 명령을 큐에 추가합니다. (Run 명령 등에서 사용)
    /// </summary>
    private unsafe bool SendIntCommand(EN_IPC_COMMAND mainCmd, int intParam)
    {
        var command = new SIpcCommCommand
        {
            m_cmd = (byte)mainCmd
        };

        // 로컬 구조체의 fixed buffer는 unsafe 내에서 바로 포인터로 사용 가능합니다.
        // 별도의 fixed 문이 필요하지 않습니다.
        byte* p = command.param;
        *(int*)p = intParam;

        return _smService.EnqueueCommand(QueueName, command);
    }

    public bool Run(int startLine = 1) => SendIntCommand(EN_IPC_COMMAND.IPC_COMMAND_RUN, startLine);
    public bool Stop() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_STOP);
    public bool Pause() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_PAUSE);
    public bool EmergencyStop() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_EMG);
    public bool Reset() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_RESET);
}
