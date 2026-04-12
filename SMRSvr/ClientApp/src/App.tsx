import { useState, useEffect } from 'react';
import axios from 'axios';

interface Position {
  x: number;
  y: number;
  z: number;
  a: number;
  b: number;
  timestamp: string;
}

function App() {
  const [position, setPosition] = useState<Position | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const interval = setInterval(async () => {
      try {
        const response = await axios.get<Position>('https://localhost:7055/api/MachineStatus/position');
        setPosition(response.data);
        setError(null);
      } catch (err) {
        setError('백엔드 서버에 연결할 수 없습니다. (https://localhost:7055)');
      }
    }, 100);

    return () => clearInterval(interval);
  }, []);

  return (
    <div style={{ padding: '40px', fontFamily: 'monospace', backgroundColor: '#1e1e1e', color: '#fff', minHeight: '100vh' }}>
      <h1 style={{ borderBottom: '2px solid #333', paddingBottom: '10px' }}>CNC Real-time Monitor</h1>
      
      {error && <div style={{ color: '#ff6b6b', marginBottom: '20px', fontSize: '14px' }}>⚠️ {error}</div>}
      
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '20px', marginTop: '20px' }}>
        <CoordinateCard label="X-Axis" value={position?.x} color="#4dabf7" />
        <CoordinateCard label="Y-Axis" value={position?.y} color="#4dabf7" />
        <CoordinateCard label="Z-Axis" value={position?.z} color="#4dabf7" />
        <CoordinateCard label="A-Axis" value={position?.a} color="#51cf66" />
        <CoordinateCard label="B-Axis" value={position?.b} color="#51cf66" />
      </div>

      <div style={{ marginTop: '40px', borderTop: '1px solid #333', paddingTop: '20px', color: '#888' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between' }}>
          <span>Status: <span style={{ color: position ? '#51cf66' : '#ff6b6b' }}>{position ? '● Connected' : '○ Offline'}</span></span>
          <span>Last Sync: {position ? new Date(position.timestamp).toLocaleTimeString() : 'N/A'}</span>
        </div>
      </div>
    </div>
  );
}

function CoordinateCard({ label, value, color }: { label: string, value?: number, color: string }) {
  return (
    <div style={{ border: '1px solid #333', padding: '20px', borderRadius: '8px', backgroundColor: '#252525', boxShadow: '0 4px 6px rgba(0,0,0,0.3)' }}>
      <div style={{ fontSize: '14px', color: '#888', marginBottom: '10px', textTransform: 'uppercase', letterSpacing: '1px' }}>{label}</div>
      <div style={{ fontSize: '36px', fontWeight: 'bold', color: color, textAlign: 'right' }}>
        {value !== undefined ? value.toFixed(4) : '0.0000'}
      </div>
    </div>
  );
}

export default App;
