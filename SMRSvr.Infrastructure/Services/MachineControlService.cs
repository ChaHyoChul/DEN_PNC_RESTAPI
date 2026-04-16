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
            // SIpcCommCommand의 UParam 구조에 맞춰 데이터 채움
            // mp (m_cmd 전용 파라미터)와 sp (s_cmd 포함 파라미터) 중 sp 형식을 주로 사용
            fixed (byte* p = command.param)
            {
                p[0] = subCmd; // s_cmd 위치
                if (!string.IsNullOrEmpty(param))
                {
                    SharedMemoryService.SetAnsiString(p + 1, param, 62); // 나머지 62바이트에 param 복사
                }
            }
        }
        
        return _smService.EnqueueCommand(QueueName, command);
    }

    public bool Run() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_RUN);
    public bool Stop() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_STOP);
    public bool Pause() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_PAUSE);
    public bool EmergencyStop() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_EMG);
    public bool Reset() => SendCommand(EN_IPC_COMMAND.IPC_COMMAND_RESET);
}
