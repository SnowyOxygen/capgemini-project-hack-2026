import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { LogOut, Plus, LayoutDashboard, Building2 } from 'lucide-react';
import axios from 'axios';
import CO2EmissionsChart, { calculateTotalCO2 } from '../components/CO2EmissionsChart';

export default function Dashboard() {
  const navigate = useNavigate();
  const [sites, setSites] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchSites();
  }, []);

  const fetchSites = async () => {
    try {
      const token = localStorage.getItem('token');
      const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/Sites`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      setSites(response.data);
      setLoading(false);
    } catch (err) {
      console.error('Error fetching sites:', err);
      setError('Failed to load sites');
      setLoading(false);
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
          <Building2 size={32} color="white" />
          <h2 style={styles.logoText}>Dashboard</h2>
        </div>
        
        <nav style={styles.nav}>
          <a href="#" style={{...styles.navItem, ...styles.navItemActive}}>
            <LayoutDashboard size={20} />
            Dashboard
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
        <div style={styles.content}>
          {/* KPI Card */}
          <div style={styles.kpiCard}>
            <div style={styles.kpiIconWrapper}>
              <Building2 size={24} color="#8DC5AA" />
            </div>
            <div style={styles.kpiInfo}>
              <h3 style={styles.kpiTitle}>Total Batiments</h3>
              <p style={styles.kpiValue}>
                {loading ? 'Loading...' : error ? 'Error loading data' : sites.length}
              </p>
            </div>
          </div>

          {/* Sites Grid */}
          {loading ? (
            <div style={styles.loadingMessage}>Loading sites...</div>
          ) : error ? (
            <div style={styles.errorMessage}>{error}</div>
          ) : sites.length === 0 ? (
            <div style={styles.emptyMessage}>No sites available. Click "Add Batiment" to create one.</div>
          ) : (
            <div style={styles.sitesGrid}>
              {sites.map(site => {
                const totalCO2 = calculateTotalCO2(site.materiaux);

                return (
                  <div key={site.id} style={styles.siteCard}>
                    <div style={styles.siteHeader}>
                      <h3 style={styles.siteName}>{site.nom}</h3>
                      <span style={styles.siteType}>{site.typeSite || 'N/A'}</span>
                    </div>
                    
                    <div style={styles.siteDetails}>
                      <div style={styles.detailItem}>
                        <span style={styles.detailLabel}>Surface:</span>
                        <span style={styles.detailValue}>{site.superficieM2 ? `${site.superficieM2.toFixed(0)} m²` : 'N/A'}</span>
                      </div>
                      <div style={styles.detailItem}>
                        <span style={styles.detailLabel}>Year:</span>
                        <span style={styles.detailValue}>{site.anneeConstruction || 'N/A'}</span>
                      </div>
                      <div style={styles.detailItem}>
                        <span style={styles.detailLabel}>Floors:</span>
                        <span style={styles.detailValue}>{site.nombreEtages || 'N/A'}</span>
                      </div>
                      <div style={styles.detailItem}>
                        <span style={styles.detailLabel}>Total CO2:</span>
                        <span style={styles.detailValue}>{totalCO2.toFixed(2)} tons</span>
                      </div>
                    </div>

                    <CO2EmissionsChart materiaux={site.materiaux} />
                  </div>
                );
              })}
            </div>
          )}
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
    backgroundColor: '#8DC5AA',
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
    maxWidth: 'none',
    boxShadow: '0 4px 6px rgba(0,0,0,0.05)',
    border: '1px solid #eaeaea',
    marginBottom: '2rem',
  },
  kpiIconWrapper: {
    backgroundColor: 'rgba(141, 197, 170, 0.1)',
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
  },
  sitesGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fill, minmax(500px, 1fr))',
    gap: '2rem',
    marginTop: '1rem',
  },
  siteCard: {
    backgroundColor: 'white',
    borderRadius: '16px',
    padding: '1.5rem',
    boxShadow: '0 4px 6px rgba(0,0,0,0.05)',
    border: '1px solid #eaeaea',
    transition: 'transform 0.2s, box-shadow 0.2s',
  },
  siteHeader: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '1rem',
    paddingBottom: '1rem',
    borderBottom: '1px solid #eaeaea',
  },
  siteName: {
    margin: 0,
    fontSize: '1.25rem',
    fontWeight: 'bold',
    color: '#333',
  },
  siteType: {
    padding: '0.25rem 0.75rem',
    backgroundColor: '#8DC5AA',
    color: 'white',
    borderRadius: '12px',
    fontSize: '0.85rem',
    fontWeight: '500',
  },
  siteDetails: {
    display: 'grid',
    gridTemplateColumns: '1fr 1fr',
    gap: '0.75rem',
    marginBottom: '1.5rem',
  },
  detailItem: {
    display: 'flex',
    justifyContent: 'space-between',
    padding: '0.5rem',
    backgroundColor: '#f8f9fa',
    borderRadius: '8px',
  },
  detailLabel: {
    fontSize: '0.875rem',
    color: '#666',
    fontWeight: '500',
  },
  detailValue: {
    fontSize: '0.875rem',
    color: '#333',
    fontWeight: 'bold',
  },
  loadingMessage: {
    textAlign: 'center',
    padding: '3rem',
    fontSize: '1.125rem',
    color: '#666',
  },
  errorMessage: {
    textAlign: 'center',
    padding: '3rem',
    fontSize: '1.125rem',
    color: '#d32f2f',
  },
  emptyMessage: {
    textAlign: 'center',
    padding: '3rem',
    fontSize: '1.125rem',
    color: '#666',
  }
};
