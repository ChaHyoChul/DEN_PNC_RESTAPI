using System.Runtime.InteropServices;
using SMRSvr.Domain.Enums;

namespace SMRSvr.Domain.Entities;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct SNCFileInfo
{
    public fixed byte id[26 * 2]; // TCHAR (Unicode)
    public byte is_select;
    public byte is_select_updown;
    public byte state;
    public byte finish;
    public uint file_size;
    public uint total_lines;
    public uint machining_lines;
    public fixed byte reversed[3 * 2]; // TCHAR (Unicode)
    public fixed byte start_time[20 * 2]; // TCHAR
    public fixed byte work_time[20 * 2]; // TCHAR
    public fixed byte file_name[257 * 2]; // TCHAR
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct SPAStatus
{
    public fixed int B_IS_RUNNING_COMPLETE[135]; // CMD_NUM = 135
    public fixed int bInput[40]; // IN_NUM = 40
    public fixed int bOutput[40]; // OUT_NUM = 40
    public int nMDCode;
    public fixed byte szCurrentFileName[128]; // char (ANSI)
    public int nBlockNumber;
    public int nLineNumber;
    public int nGPLErrorCode;
    public int nRunStatus;
    public int nNumberProgramCycle;
    public double fProgramCycleTime;
    public fixed double fPosition[5]; // AXIS_NUM = 5
    public fixed double fVelocity[5];
    public fixed double fPositionTool[5];
    public double fAutoLoaderPosition;
    public double fAutoLoaderVelocity;
    public int nServoPower;
    public int nServoHomeState;
    public int nServoErrorState;
    public int nZReadyState;
    public int nIO_Board_Status;
    public int nSpindle_Board_Status;
    public int nCurrentToolNo;
    public int nCurrentCoordinateNo;
    public double fCurrentToolLenght;
    public int nToolLengthUpdateFlag;
    public int nDuringToolChaneFlag;
    public int nSpindleRun;
    public int nSpindleSpeed;
    public int nSpindleOverride;
    public int nSpindleSpeedWithOverride;
    public int nMotorOverride;
    public int nMotorFeedrate;
    public int nMotorFeedrateWithOverride;
    public int nRndErrorCode;
    public fixed byte szRndErrorMessage[256]; // char (ANSI)
    public int nStreamStatusCode;
    public int nStreamLineNumber;
    public int nStreamBufferCount;
    public int nEMOButtonFlag;
    public int nMotorMovingFlag;
    public int nMotorMovingFlagAutoloader;
    public int nM00CommandFlag;
    public int nSpindleClamp;
    public int nPurgeAirOnOff;
    public int nAirRecharge;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct SThreadState
{
    public EN_CONNECT_STATUS hConnectStatus;
    public EN_RUNMODE hRunMode;
    public int bIsOpenNCFile;
    public SNCFileInfo hNCFileInfo;
    public int nNCFileInfo_FirstToolChageLine;
    public int nNCFileInfo_SecondToolChageLine;
    public uint dwRunningTime;
    public uint dwRunningTimeTickCount;
    public uint dwRunningTimeErrorCount;
    public uint dwFirstHalfRunningTime;
    public uint dwSecondHalfRunningTime;
    public uint dwRemainRunningTime;
    public int bIpcUpDownLoadComplete_;
    public int bIpcCmdComplete_;
    public int bIsOriginComplete_;
    public int bIsDemoMode_;
    public int bIsClientConnected_;
    public int bRemoteLock_;
    public int bRemoteAutoUpdate_;
    public int bSendRegistered_NCFileList_;
    public int bSendSDMemory_NCFileList_;
    public int bHideErrorMsgDialog_;
    public int bShowSetupToolDlg_;
    public int bUpdateNcFileList_;
    public int bUpdateNcFileListForSD_;
    public fixed uint dwLastUsedToolChangeLineNo_[100];
    public int nLastUsedToolChangeLineNoIndex_;
    public int nErrorType;
    public int nErrorCode;
    public fixed byte szErrorType[128 * 2]; // TCHAR
    public int nErrorTypeIsAlarm;
    public fixed byte szErrorCode[512 * 2]; // TCHAR
    public fixed byte szErrorMessage[2048 * 2]; // TCHAR
    public fixed byte szResponseTerminalCommand_[128]; // char (ANSI)
    public fixed byte szMotionProgVersion[128 * 2]; // TCHAR
    public fixed byte szUIProgVersion[128 * 2];
    public fixed byte szFileReceiverVersion[128 * 2];
    public fixed byte szFileReceiverVersion2[128 * 2];
    public fixed byte szPAControllerVersion[128 * 2];
    public fixed byte szPA_IP_ARRD[32 * 2];
    public fixed byte szCANTOPS_IP_ADDR[32 * 2];
    public fixed byte szNEW_PA_IP_ADDR[32]; // char
    public fixed byte szNEW_CANTOPS_IP_ADDR[32]; // char
    public int isPAIPChanged;
    public int isIOIPChanged;
    public int bIsShowUserConfirmDlg;
    public int bIsShowCableConnectDlg;
    public int bIsShowOriginDlg;
    public int bIsFileOpening;
    public fixed double fSoftLimit_[12];
    public uint dwTOTAL_SPINDLE_RUN_TIME;
    public uint dwCLEAN_SPINDLE_RUN_TIME;
    public fixed uint dwBackupToolMatrix_[400];
    public fixed byte szIpAddress[64 * 2]; // TCHAR
    public int nNcFileLoadingRate;
    public int nBufferingLine;
    public int nStartingNCCodeStepNo;
    public int nCurrentNCCodeStepNo;
    public int nNumberOfPreparingStep;
    public int nRunMode_StepNo;
    public int hUserMode;
    public int nPAYear;
    public int nPAMonth;
    public int nPADay;
    public fixed int nAutoCal_CheckBoxState[10];
    public fixed int nAutoCal_CheckingItem[10];
    public int hAutoCal_RotateAxis;
    public int nAutoCal_ConnectedCable;
    public int nAutoTeach_MultiOrigin_CoordIndex;
    public int nAutoTeach_MultiOrigin_CoordinateNo;
    public int nAutoTeach_MultiOrigin_NumRoundBar;
    public fixed int nAutoTeach_MultiOrigin_Sel_RoundBar[10];
    public fixed double fAutoTeach_MultiOrigin_Deviation[30];
    public int bNoNeedPassword;
    public int bOperationScreen_ToolButtonPressed;
    public int bOperationScreen_UsbMemory;
    public int bCheckStatus;
    public int nPauseByDoorOpen;
    public fixed int bATCTest_EnaTool[20];
    public int nATCTest_MeasureCount;
    public int nATCTest_WorkCount;
    public fixed double fATCTest_MeasureResult[600];
    public int nBarcode_Data;
    public int bCompleteResetOrigin_;
    public int bIsReceivedPNCID_;
    public fixed byte szPNCID_[64 * 2]; // TCHAR
    public int bSaveSelectedLogFiles_;
    public fixed byte szSelectedLogFiles_[1024 * 6];
    public int nNumSelectLogFiles_;
    public int nLenSelectLogFiles_;
    public int nJogSpeed_;
    public int nToolPocketAutoTeachingStepCount;
    public fixed byte szOpenedNCFileName[256 * 2]; // TCHAR
    public int bNotUsedAutoloader_For_MultiOrg_;
    public int nM28TypeRunning_;
    public int bIsManualMoving_;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct SConfigData
{
    public fixed double fCoordOffset[7 * 5]; 
    public fixed double fTeachingPoint[25 * 5]; 
    public fixed double fOptionData[5]; 
    public int bUsingDetectBlock;
    public int bUsingAirLimitSensor;
    public int nDelayGripBlock;
    public int nAirLimitInterval;
    public int nToolErrorOccure_HandlingCode;
    public int bUsingOpPanel;
    public int bUsingFlowSensor;
    public int nFlowSensorTimeout;
    public int nFlowSensorStartTimeout;
    public int bCheckInvalidNcCode;
    public int bTransformNcFile;
    public int bCheckNcFileTag;
    public int bAutoSelectM28Type;
    public int bUsingSpindleAirPurge;
    public int nEnableOperationLog;
    public int nEnableIpcCommLog;
    public int nEnableThreadModeLog;
    public int nEnableOpPenalLog;
    public int nEnableExtLog;
    public int nEnableErrLog;
    public fixed double fCoordinateOffset[5];
    public int nPurgeLimitInterval;
    public int nPurgeAirHoldTime;
    public int nSelectM28Operation;
    public int nAirPurge_AutoOff_Time_Minute;
    public int nToolPocket_Shape;
    public int bCheckMachineID;
    public int bUseMultipleReadyPos;
    public int bConfirmBeforeMoving;
    public int bUsingAutoloaderSensor;
    public int bUsingAutoloaderAirBlow;
    public int bUsingAutoloaderInitDuringClean_;
    public int nCleaningTimeout_Hour;
    public fixed byte szM00CommandMessage[128 * 2]; // TCHAR
    public int nMultiOriginType;
    public int bFlashWrite;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct STool
{
    public uint dwMaximumTime;
    public uint dwUsingTime;
    public double fUsingRate;
    public uint dwErrCode;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct SToolData
{
    public int bEnableToolUsageTime;
    public int bEnableRelatedTool;
    public double fTimeCountZAxisPos;
    public fixed byte hTool[20 * 24]; // MAX_TOOL_NUM=20, sizeof(STool)=24
    public int nNumDataForRelatedTool;
    public fixed byte szRelateTool[21 * 64]; // char[21][64] (ANSI)
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct SAutoLoaderBlockState
{
    public fixed int hBlockState[12]; 
    public fixed int slotHolderNumber[12];
    public int nWorkIndex;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct SAutoLoaderConfig
{
    public int bUsingAutoLoader;
    public int nNumBlock_;
    public double fScale_;
    public fixed double fTeachingPoint_[15]; 
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct ItemLocationState
{
    public int autoLoaderHandItemNumber;
    public int itemPasserItemNumber;
    public int workSpaceItemNumber;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct SMultiOriginHolderInfo
{
    public int nNumOfHolder;
    public fixed int nHolderNumber[20];
    public fixed byte szHolderFileName[20 * 64 * 2]; // TCHAR szHolderFileName[20][64]
    public int nSelectedIndex;
    public IntPtr pRefMOD_; // SMultiOriginData*
    public IntPtr pRefATMOP_; // SAutoTeachMultiOrgParam*
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct SMultiOriginData
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct _MultiOrigin
    {
        public int nGcode;
        public fixed double fOffset[20 * 6]; // double[20][6]
        public fixed int bReverseXY[20]; // BOOL[20]
        public fixed int nAbutmentArea[20 * 4]; // int[20][4]
        public fixed byte szImageFilename[64 * 2]; // TCHAR[64]
    }

    public int nNumGCode;
    public fixed byte hMultiOrigin[4 * 1048]; // _MultiOrigin[4]. size of _MultiOrigin is approx 1048
    public int nNumAbutment;
    public double fOffset_Min_;
    public double fOffset_Max_;
    public fixed byte szFilanameForAutoTeachParam[512 * 2]; // TCHAR[512]
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct SAutoCalCoordinateOffsetParam
{
    public fixed double fParam[21]; // PARAM_NUM=21
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct SAutoTeachToolPocketParam
{
    public fixed double fParam[24]; // PARAM_NUM=24
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct SAutoTeachMultiOrgParam
{
    public fixed double fParam[43]; // PARAM_NUM=43
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct SCoordinateOffsetDataRange
{
    public fixed double fDefault[5]; // AXIS_NUM=5
    public fixed double fMax[5];
    public fixed double fMin[5];
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct SMeasureParamEtc
{
    public fixed int nUsingTool[16];
    public double fDefaultToolDiameter;
    public double fMinToolDiameter;
    public double fMaxToolDiameter;
    public double fDefaultDiskThickness;
    public double fMinDiskThickness;
    public double fMaxDiskThickness;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct SIpcCommCommand
{
    public byte m_cmd;
    public fixed byte param[63];
}
