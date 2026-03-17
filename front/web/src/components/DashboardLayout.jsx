import { useNavigate, Outlet, useLocation, useParams } from 'react-router-dom';
import { LogOut, Plus, LayoutDashboard } from 'lucide-react';
import { useState, useEffect } from 'react';
import axios from 'axios';

export default function DashboardLayout() {
  const navigate = useNavigate();
  const location = useLocation();
  const params = useParams();
  const [currentSite, setCurrentSite] = useState(null);
  
  const isOnSiteDetail = location.pathname.startsWith('/site/');
  
  useEffect(() => {
    if (params.id) {
      fetchSiteName(params.id);
    } else {
      setCurrentSite(null);
    }
  }, [params.id]);

  const fetchSiteName = async (id) => {
    try {
      const token = localStorage.getItem('token');
      const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/Sites`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      const site = response.data.find(s => s.id === parseInt(id));
      if (site) {
        setCurrentSite(site);
      }
    } catch (err) {
      console.error('Error fetching site:', err);
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/');
  };

  const handleAddBatiment = () => {
    navigate('/add-site');
  };

  return (
    <div style={styles.container}>
      {/* Sidebar */}
      <aside style={styles.sidebar}>
        <div style={styles.logoArea}>
          <LayoutDashboard size={32} color="white" />
          <h2 style={styles.logoText}>Sites</h2>
        </div>
        
        <nav style={styles.nav}>
          {isOnSiteDetail && currentSite ? (
            <button 
              onClick={() => navigate(`/site/${currentSite.id}`)} 
              style={{
                ...styles.navItem, 
                ...styles.navItemActive,
                border: 'none', 
                background: 'none', 
                width: '100%', 
                textAlign: 'left', 
                cursor: 'pointer'
              }}
            >
              <LayoutDashboard size={20} />
              {currentSite.nom}
            </button>
          ) : (
            <button 
              onClick={() => navigate('/dashboard')} 
              style={{
                ...styles.navItem, 
                ...styles.navItemActive,
                border: 'none', 
                background: 'none', 
                width: '100%', 
                textAlign: 'left', 
                cursor: 'pointer'
              }}
            >
              <LayoutDashboard size={20} />
              Dashboard
            </button>
          )}
        </nav>

        <div style={styles.sidebarBottom}>
          <button style={styles.actionButton} onClick={handleAddBatiment}>
            <Plus size={20} />
            <span>Add Site</span>
          </button>
          
          <button style={styles.logoutButton} onClick={handleLogout}>
            <LogOut size={20} />
            <span>Logout</span>
          </button>
        </div>
      </aside>

      {/* Main Content - This is where child routes will render */}
      <main style={styles.main}>
        <Outlet />
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
    backgroundColor: '#8DC5AA',
    color: 'white',
    display: 'flex',
    flexDirection: 'column',
    padding: '2rem 0',
    boxShadow: '4px 0 10px rgba(0,0,0,0.1)',
    position: 'fixed',
    top: 0,
    left: 0,
    height: '100vh',
    overflowY: 'auto',
  },
  logoArea: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.75rem',
    marginBottom: '3rem',
    padding: '0 1.5rem',
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
    gap: '0rem',
  },
  navItem: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.75rem',
    padding: '0.8rem 1.5rem',
    borderRadius: '0',
    color: 'white',
    textDecoration: 'none',
    transition: 'all 0.2s',
    fontWeight: '500',
    backgroundColor: 'transparent',
  },
  navItemActive: {
    backgroundColor: '#66A688',
    color: 'white',
    fontWeight: '600',
  },
  sidebarBottom: {
    display: 'flex',
    flexDirection: 'column',
    gap: '1rem',
    padding: '0 1.5rem',
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
    overflow: 'auto',
    marginLeft: '260px',
  },
};
