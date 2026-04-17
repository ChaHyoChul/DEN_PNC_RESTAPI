import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import Monitor from './components/Monitor';
import ApiTester from './components/ApiTester';
import './App.css';

function App() {
  return (
    <Router>
      <div style={{ minHeight: '100vh', backgroundColor: '#1e1e1e', color: '#fff', fontFamily: 'monospace' }}>
        <nav style={{ 
          padding: '10px 20px', 
          backgroundColor: '#252525', 
          borderBottom: '1px solid #333',
          display: 'flex',
          gap: '20px'
        }}>
          <Link to="/" style={navLinkStyle}>Monitor</Link>
          <Link to="/api-test" style={navLinkStyle}>API Tester</Link>
        </nav>

        <main style={{ padding: '20px' }}>
          <Routes>
            <Route path="/" element={<Monitor />} />
            <Route path="/api-test" element={<ApiTester />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

const navLinkStyle = {
  color: '#4dabf7',
  textDecoration: 'none',
  fontSize: '16px',
  fontWeight: 'bold'
};

export default App;
