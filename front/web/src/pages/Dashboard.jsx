import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { LogOut, Plus, LayoutDashboard, Building2 } from 'lucide-react';

export default function Dashboard() {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/');
  };

  const handleAddBatiment = () => {
    alert("Add Batiment Action Triggered!");
  };

  return (
    <div style={styles.container}>
      {/* Sidebar */}
      <aside style={styles.sidebar}>
        <div style={styles.logoArea}>
          <Building2 size={32} color="white" />
          <h2 style={styles.logoText}>MyApp</h2>
        </div>
        
        <nav style={styles.nav}>
          <a href="#" style={{...styles.navItem, ...styles.navItemActive}}>
            <LayoutDashboard size={20} />
            Overview
          </a>
        </nav>

        <div style={styles.sidebarBottom}>
          <button style={styles.actionButton} onClick={handleAddBatiment}>
            <Plus size={20} />
            <span>Add Batiment</span>
          </button>
          
          <button style={styles.logoutButton} onClick={handleLogout}>
            <LogOut size={20} />
            <span>Logout</span>
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <main style={styles.main}>
        <header style={styles.header}>
          <h1 style={styles.pageTitle}>Dashboard Overview</h1>
        </header>

        <div style={styles.content}>
          {/* KPI Card */}
          <div style={styles.kpiCard}>
            <div style={styles.kpiIconWrapper}>
              <Building2 size={24} color="#1e3c72" />
            </div>
            <div style={styles.kpiInfo}>
              <h3 style={styles.kpiTitle}>Total Batiments</h3>
              <p style={styles.kpiValue}>No data available</p>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}

const styles = {
  container: {
    display: 'flex',
    minHeight: '100vh',
    fontFamily: '"Inter", sans-serif',
    backgroundColor: '#f4f7f6',
  },
  sidebar: {
    width: '260px',
    backgroundColor: '#1e3c72',
    color: 'white',
    display: 'flex',
    flexDirection: 'column',
    padding: '2rem 1.5rem',
    boxShadow: '4px 0 10px rgba(0,0,0,0.1)',
  },
  logoArea: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.75rem',
    marginBottom: '3rem',
  },
  logoText: {
    margin: 0,
    fontSize: '1.5rem',
    fontWeight: 'bold',
    letterSpacing: '1px',
  },
  nav: {
    flex: 1,
    display: 'flex',
    flexDirection: 'column',
    gap: '0.5rem',
  },
  navItem: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.75rem',
    padding: '0.8rem 1rem',
    borderRadius: '8px',
    color: 'rgba(255,255,255,0.7)',
    textDecoration: 'none',
    transition: 'all 0.2s',
    fontWeight: '500',
  },
  navItemActive: {
    backgroundColor: 'rgba(255,255,255,0.15)',
    color: 'white',
  },
  sidebarBottom: {
    display: 'flex',
    flexDirection: 'column',
    gap: '1rem',
  },
  actionButton: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    gap: '0.5rem',
    padding: '0.8rem',
    backgroundColor: '#4caf50',
    color: 'white',
    border: 'none',
    borderRadius: '8px',
    fontSize: '1rem',
    fontWeight: 'bold',
    cursor: 'pointer',
    transition: 'background 0.2s',
  },
  logoutButton: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    gap: '0.5rem',
    padding: '0.8rem',
    backgroundColor: 'transparent',
    color: 'rgba(255,255,255,0.7)',
    border: '1px solid rgba(255,255,255,0.3)',
    borderRadius: '8px',
    fontSize: '1rem',
    fontWeight: 'bold',
    cursor: 'pointer',
    transition: 'all 0.2s',
  },
  main: {
    flex: 1,
    display: 'flex',
    flexDirection: 'column',
  },
  header: {
    backgroundColor: 'white',
    padding: '2rem 3rem',
    borderBottom: '1px solid #eaeaea',
  },
  pageTitle: {
    margin: 0,
    fontSize: '1.8rem',
    color: '#333',
  },
  content: {
    padding: '3rem',
    flex: 1,
  },
  kpiCard: {
    backgroundColor: 'white',
    borderRadius: '16px',
    padding: '2rem',
    display: 'flex',
    alignItems: 'center',
    gap: '1.5rem',
    maxWidth: '400px',
    boxShadow: '0 4px 6px rgba(0,0,0,0.05)',
    border: '1px solid #eaeaea',
  },
  kpiIconWrapper: {
    backgroundColor: 'rgba(30, 60, 114, 0.1)',
    width: '64px',
    height: '64px',
    borderRadius: '16px',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
  },
  kpiInfo: {
    display: 'flex',
    flexDirection: 'column',
  },
  kpiTitle: {
    margin: '0 0 0.25rem 0',
    fontSize: '1rem',
    color: '#888',
    fontWeight: '500',
  },
  kpiValue: {
    margin: 0,
    fontSize: '1.25rem',
    color: '#333',
    fontWeight: 'bold',
  }
};
