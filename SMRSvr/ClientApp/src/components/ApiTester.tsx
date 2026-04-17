import { useState } from 'react';
import axios from 'axios';

const BASE_URL = '';

export default function ApiTester() {
  const [response, setResponse] = useState<any>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const callApi = async (method: string, endpoint: string, params?: any) => {
    setLoading(true);
    setError(null);
    setResponse(null);
    try {
      const res = await axios({
        method,
        url: `${BASE_URL}${endpoint}`,
        params,
      });
      setResponse(res.data);
    } catch (err: any) {
      setError(err.response?.data || err.message);
      setResponse(err.response?.data);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ padding: '20px', fontFamily: 'monospace' }}>
      <h1>REST API Tester</h1>
      
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '20px' }}>
        <div>
          <section style={sectionStyle}>
            <h3>Machine Control</h3>
            <div style={buttonGroupStyle}>
              <button onClick={() => callApi('POST', '/pnc/MachineControl/run')}>Run</button>
              <button onClick={() => callApi('POST', '/pnc/MachineControl/stop')}>Stop</button>
              <button onClick={() => callApi('POST', '/pnc/MachineControl/pause')}>Pause</button>
              <button onClick={() => callApi('POST', '/pnc/MachineControl/reset')}>Reset</button>
              <button onClick={() => callApi('POST', '/pnc/MachineControl/emergency-stop')} style={{ backgroundColor: '#c92a2a' }}>Emergency Stop</button>
            </div>
          </section>

          <section style={sectionStyle}>
            <h3>Machine Status</h3>
            <div style={buttonGroupStyle}>
              <button onClick={() => callApi('GET', '/pnc/MachineStatus/status')}>Get Status</button>
              <button onClick={() => callApi('GET', '/pnc/MachineStatus/position')}>Get Position</button>
              <button onClick={() => callApi('GET', '/pnc/MachineStatus/tool-status')}>Get Tool Status</button>
              <button onClick={() => callApi('GET', '/pnc/MachineStatus/io')}>Get IO</button>
              <button onClick={() => callApi('GET', '/pnc/MachineStatus/error')}>Get Error</button>
            </div>
          </section>

          <section style={sectionStyle}>
            <h3>NC File Manage</h3>
            <div style={buttonGroupStyle}>
              <button onClick={() => callApi('GET', '/pnc/NcFileManage/list')}>List Files</button>
              <button onClick={() => callApi('POST', '/pnc/NcFileManage/close')}>Close File</button>
            </div>
            <div style={{ marginTop: '10px' }}>
              <input id="fileName" placeholder="File Name (e.g. test.nc)" style={inputStyle} />
              <button onClick={() => callApi('POST', '/pnc/NcFileManage/open', { fileName: (document.getElementById('fileName') as HTMLInputElement).value })}>Open File</button>
            </div>
          </section>
        </div>

        <div style={{ backgroundColor: '#252525', padding: '20px', borderRadius: '8px', border: '1px solid #333', overflow: 'auto', maxHeight: '80vh' }}>
          <h3>Response</h3>
          {loading && <div>Loading...</div>}
          {error && <div style={{ color: '#ff6b6b' }}>Error: {typeof error === 'string' ? error : JSON.stringify(error)}</div>}
          {response && (
            <pre style={{ color: '#51cf66', whiteSpace: 'pre-wrap', wordBreak: 'break-all' }}>
              {JSON.stringify(response, null, 2)}
            </pre>
          )}
        </div>
      </div>
    </div>
  );
}

const sectionStyle = {
  marginBottom: '20px',
  padding: '15px',
  border: '1px solid #333',
  borderRadius: '8px',
  backgroundColor: '#252525'
};

const buttonGroupStyle = {
  display: 'flex',
  flexWrap: 'wrap' as 'wrap',
  gap: '10px'
};

const inputStyle = {
  backgroundColor: '#1a1a1a',
  color: '#fff',
  border: '1px solid #444',
  padding: '5px 10px',
  marginRight: '10px',
  borderRadius: '4px'
};
