import { useState, useEffect, useCallback } from 'react';
import axios from 'axios';

interface MachineStatus {
  runMode: string;
  position: number[];
  inputs: boolean[];
  outputs: boolean[];
  currentToolNo: number;
  isNcFileLoaded: boolean;
  ncFileName: string;
  totalLines: number;
  currentLine: number;
  errorType: number;
  errorCode: number;
  timestamp: string;
}

interface NcFile {
  id: string;
  fileName: string;
  fileSize: number;
  state: string;
  isFinished: boolean;
  isSelected: boolean;
}

interface NcFileList {
  totalCount: number;
  files: NcFile[];
}

const BASE_URL = '';

export default function Monitor() {
  const [status, setStatus] = useState<MachineStatus | null>(null);
  const [ncFiles, setNcFiles] = useState<NcFile[]>([]);
  const [selectedFileName, setSelectedFileName] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const fetchStatus = useCallback(async () => {
    try {
      const response = await axios.get<MachineStatus>(`${BASE_URL}/pnc/MachineStatus/status`);
      setStatus(response.data);
    } catch (err) {
      setError('백엔드 서버에 연결할 수 없습니다. (Status)');
    }
  }, []);

  const fetchFileList = useCallback(async () => {
    try {
      const response = await axios.get<NcFileList>(`${BASE_URL}/pnc/NcFileManage/list`);
      setNcFiles(response.data.files || []);
    } catch (err) {
      console.error('Failed to fetch file list', err);
    }
  }, []);

  useEffect(() => {
    const statusInterval = setInterval(fetchStatus, 500);
    const listInterval = setInterval(fetchFileList, 2500);
    return () => {
      clearInterval(statusInterval);
      clearInterval(listInterval);
    };
  }, [fetchStatus, fetchFileList]);

  const handleRun = async () => {
    if (!window.confirm('가공을 시작하시겠습니까?')) return;
    try {
      await axios.post(`${BASE_URL}/pnc/MachineControl/run`);
    } catch (err: any) {
      alert(err.response?.data || '가공 시작 명령 실패');
    }
  };

  const handleStop = async () => {
    if (!window.confirm('가공을 중지하시겠습니까?')) return;
    try {
      await axios.post(`${BASE_URL}/pnc/MachineControl/stop`);
    } catch (err: any) {
      alert(err.response?.data || '가공 중지 명령 실패');
    }
  };

  const handlePause = async () => {
    if (!window.confirm('가공을 일시 정지하시겠습니까?')) return;
    try {
      await axios.post(`${BASE_URL}/pnc/MachineControl/pause`);
    } catch (err: any) {
      alert(err.response?.data || '일시 정지 명령 실패');
    }
  };

  const handleEmergency = async () => {
    try {
      await axios.post(`${BASE_URL}/pnc/MachineControl/emergency-stop`);
    } catch (err: any) {
      alert(err.response?.data || '비상 정지 명령 실패');
    }
  };

  const handleReset = async () => {
    try {
      await axios.post(`${BASE_URL}/pnc/MachineControl/reset`);
    } catch (err: any) {
      alert(err.response?.data || '리셋 명령 실패');
    }
  };

  const handleOpen = async () => {
    if (!selectedFileName) return alert('파일을 선택해주세요.');
    if (!window.confirm(`선택한 파일(${selectedFileName})을 오픈하시겠습니까?`)) return;
    try {
      await axios.post(`${BASE_URL}/pnc/NcFileManage/open`, null, { params: { fileName: selectedFileName } });
    } catch (err: any) {
      alert(err.response?.data?.message || '파일 오픈에 실패했습니다.');
    }
  };

  const handleClose = async () => {
    if (!window.confirm('현재 열려있는 파일을 닫으시겠습니까?')) return;
    try {
      await axios.post(`${BASE_URL}/pnc/NcFileManage/close`);
    } catch (err: any) {
      alert(err.response?.data?.message || '파일 클로즈에 실패했습니다.');
    }
  };

  const handleRemove = async () => {
    if (!selectedFileName) return alert('삭제할 파일을 선택해주세요.');
    if (!window.confirm(`정말로 ${selectedFileName} 파일을 삭제하시겠습니까?`)) return;
    try {
      await axios.delete(`${BASE_URL}/pnc/NcFileManage/remove`, { params: { fileName: selectedFileName } });
      setSelectedFileName(null);
      fetchFileList();
    } catch (err: any) {
      alert(err.response?.data?.message || '파일 삭제에 실패했습니다.');
    }
  };

  const handleAdd = async () => {
    const path = window.prompt('추가할 NC 파일의 절대 경로를 입력하세요:');
    if (!path) return;
    try {
      await axios.post(`${BASE_URL}/pnc/NcFileManage/add`, null, { params: { fullPath: path } });
      fetchFileList();
    } catch (err: any) {
      alert(err.response?.data?.message || '파일 추가에 실패했습니다.');
    }
  };

  return (
    <div style={{ padding: '20px', fontFamily: 'monospace' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', borderBottom: '2px solid #333', paddingBottom: '10px', marginBottom: '20px' }}>
        <h1 style={{ margin: 0, color: '#74c0fc', fontSize: '36px' }}>PNC Real-time Monitor</h1>
        <div style={{ 
          padding: '5px 15px', 
          borderRadius: '20px', 
          backgroundColor: getStatusColor(status?.runMode),
          color: '#000',
          fontWeight: 'bold',
          fontSize: '14px'
        }}>
          {status?.runMode || 'OFFLINE'}
        </div>
      </div>
      
      {error && <div style={{ color: '#ff6b6b', marginBottom: '20px', fontSize: '14px' }}>⚠️ {error}</div>}
      
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(5, 1fr)', gap: '10px', marginBottom: '20px' }}>
        <CoordinateCard label="X" value={status?.position?.[0]} color="#4dabf7" />
        <CoordinateCard label="Y" value={status?.position?.[1]} color="#4dabf7" />
        <CoordinateCard label="Z" value={status?.position?.[2]} color="#4dabf7" />
        <CoordinateCard label="A" value={status?.position?.[3]} color="#51cf66" />
        <CoordinateCard label="B" value={status?.position?.[4]} color="#51cf66" />
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '20px', marginBottom: '20px' }}>
        <InfoSection title="File Information">
          <InfoItem label="NC File" value={status?.ncFileName || 'None'} />
          <InfoItem label="Progress" value={(status?.currentLine !== undefined && status?.totalLines !== undefined) ? `${status.currentLine} / ${status.totalLines}` : '0 / 0'} />
          <InfoItem label="Tool No" value={status?.currentToolNo?.toString() || '0'} />
        </InfoSection>

        <InfoSection title="System Status">
          <InfoItem 
            label="Error Type/Code" 
            value={(status?.runMode === 'RUNMODE_ERROR') ? `${status.errorType} / ${status.errorCode}` : '0 / 0'} 
            color={(status?.runMode === 'RUNMODE_ERROR') ? '#ff6b6b' : '#fff'} 
          />
          <InfoItem label="Sync Time" value={status ? new Date(status.timestamp).toLocaleTimeString() : 'N/A'} />
        </InfoSection>
      </div>

      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '20px', backgroundColor: '#252525', padding: '15px', borderRadius: '8px', border: '1px solid #333' }}>
        <div style={{ display: 'flex', gap: '15px' }}>
          <button onClick={handleRun} style={{...controlBtnStyle, backgroundColor: '#2f9e44'}}>RUN</button>
          <button onClick={handlePause} style={{...controlBtnStyle, backgroundColor: '#fcc419', color: '#000'}}>PAUSE</button>
          <button onClick={handleStop} style={{...controlBtnStyle, backgroundColor: '#e03131'}}>STOP</button>
        </div>
        <div style={{ display: 'flex', gap: '15px' }}>
          <button onClick={handleEmergency} style={{...controlBtnStyle, backgroundColor: '#c92a2a', minWidth: '150px'}}>Emergency STOP</button>
          <button onClick={handleReset} style={{...controlBtnStyle, backgroundColor: '#1971c2'}}>RESET</button>
        </div>
      </div>

      <div style={{ backgroundColor: '#252525', padding: '15px', borderRadius: '8px', border: '1px solid #333' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '15px', borderBottom: '1px solid #444', paddingBottom: '10px' }}>
          <h3 style={{ margin: 0 }}>NC File Management</h3>
          <div style={{ display: 'flex', gap: '8px' }}>
            <button onClick={handleAdd} style={btnStyle}>ADD</button>
            <button onClick={handleRemove} style={{...btnStyle, backgroundColor: '#c92a2a'}}>REMOVE</button>
            <button onClick={handleOpen} style={{...btnStyle, backgroundColor: '#2f9e44'}}>OPEN</button>
            <button onClick={handleClose} style={{...btnStyle, backgroundColor: '#e67700'}}>CLOSE</button>
          </div>
        </div>

        <div style={{ maxHeight: '200px', overflowY: 'auto' }}>
          <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left', fontSize: '13px' }}>
            <thead>
              <tr style={{ borderBottom: '1px solid #444' }}>
                <th style={{ padding: '8px' }}>File Name</th>
                <th style={{ padding: '8px' }}>Size (KB)</th>
                <th style={{ padding: '8px' }}>State</th>
                <th style={{ padding: '8px' }}>Finished</th>
              </tr>
            </thead>
            <tbody>
              {ncFiles.length > 0 ? (
                ncFiles.map(file => (
                  <tr 
                    key={file.fileName}
                    onClick={() => setSelectedFileName(file.fileName)}
                    style={{ 
                      cursor: 'pointer',
                      backgroundColor: selectedFileName === file.fileName ? '#333' : 'transparent',
                      borderBottom: '1px solid #333'
                    }}
                  >
                    <td style={{ padding: '8px' }}>{file.fileName}</td>
                    <td style={{ padding: '8px' }}>{(file.fileSize / 1024).toFixed(1)}</td>
                    <td style={{ padding: '8px' }}>{file.state}</td>
                    <td style={{ padding: '8px' }}>{file.isFinished ? '✅' : '❌'}</td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan={4} style={{ padding: '20px', textAlign: 'center', color: '#888' }}>No files in list</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

const btnStyle = {
  padding: '6px 12px',
  border: 'none',
  borderRadius: '4px',
  backgroundColor: '#1971c2',
  color: '#fff',
  fontWeight: 'bold',
  cursor: 'pointer',
  fontSize: '12px'
};

const controlBtnStyle = {
  padding: '10px 20px',
  border: 'none',
  borderRadius: '4px',
  color: '#fff',
  fontWeight: 'bold',
  cursor: 'pointer',
  fontSize: '14px',
  minWidth: '100px'
};

function getStatusColor(mode?: string) {
  switch (mode) {
    case 'RUNMODE_RUN': return '#51cf66';
    case 'RUNMODE_PAUSE': return '#fcc419';
    case 'RUNMODE_STOP': return '#adb5bd';
    case 'RUNMODE_ERROR': return '#ff6b6b';
    default: return '#495057';
  }
}

function CoordinateCard({ label, value, color }: { label: string, value?: number, color: string }) {
  return (
    <div style={{ border: '1px solid #333', padding: '8px 12px', borderRadius: '4px', backgroundColor: '#252525', display: 'flex', justifyContent: 'space-between', alignItems: 'baseline' }}>
      <div style={{ fontSize: '14px', fontWeight: 'bold', color: '#888', textTransform: 'uppercase' }}>{label}</div>
      <div style={{ fontSize: '18px', fontWeight: 'bold', color: color }}>
        {value !== undefined ? value.toFixed(3) : '0.000'}
      </div>
    </div>
  );
}

function InfoSection({ title, children }: { title: string, children: React.ReactNode }) {
  return (
    <div style={{ backgroundColor: '#252525', padding: '15px', borderRadius: '8px', border: '1px solid #333' }}>
      <h3 style={{ marginTop: 0, borderBottom: '1px solid #444', paddingBottom: '10px', fontSize: '16px' }}>{title}</h3>
      {children}
    </div>
  );
}

function InfoItem({ label, value, color = '#fff' }: { label: string, value: string, color?: string }) {
  return (
    <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '8px', fontSize: '14px' }}>
      <span style={{ color: '#888' }}>{label}</span>
      <span style={{ color: color, fontWeight: 'bold' }}>{value}</span>
    </div>
  );
}
