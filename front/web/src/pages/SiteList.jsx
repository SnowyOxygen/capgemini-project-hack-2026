import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { ChevronLeft, Building2, LayoutGrid, List as ListIcon, Search, AlertCircle, TrendingUp, TrendingDown } from 'lucide-react';

export default function SiteList() {
  const navigate = useNavigate();
  const [sites, setSites] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    fetchSites();
  }, []);

  const fetchSites = async () => {
    try {
      const token = localStorage.getItem('token');
      const response = await axios.get('http://localhost:5000/api/sites', {
        headers: { Authorization: `Bearer ${token}` },
      });
      setSites(response.data);
    } catch (err) {
      setError('Failed to fetch buildings. Please try again later.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const filteredSites = sites.filter(site => 
    site.nom.toLowerCase().includes(searchTerm.toLowerCase()) ||
    site.typeSite?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div style={styles.container}>
      <header style={styles.header}>
        <div style={styles.headerLeft}>
          <button onClick={() => navigate('/dashboard')} style={styles.backButton}>
            <ChevronLeft size={20} />
            Back to Dashboard
          </button>
          <h2 style={styles.title}>Site List</h2>
        </div>
        
        <div style={styles.searchContainer}>
          <Search size={18} color="#888" />
          <input 
            type="text" 
            placeholder="Search buildings..." 
            style={styles.searchInput}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>
      </header>

      {loading ? (
        <div style={styles.centerContent}>
          <div style={styles.loader}></div>
          <p>Loading buildings...</p>
        </div>
      ) : error ? (
        <div style={styles.centerContent}>
          <AlertCircle size={48} color="#ff4d4f" />
          <p style={styles.errorText}>{error}</p>
          <button onClick={fetchSites} style={styles.retryButton}>Retry</button>
        </div>
      ) : (
        <div style={styles.content}>
          <div style={styles.tableCard}>
            <table style={styles.table}>
              <thead>
                <tr>
                  <th style={styles.th}>Building Name</th>
                  <th style={styles.th}>Type</th>
                  <th style={styles.th}>Surface</th>
                  <th style={styles.th}>Construction</th>
                  <th style={styles.th}>Annual (tCO2e/an)</th>
                </tr>
              </thead>
              <tbody>
                {filteredSites.map((site) => (
                  <tr key={site.id} style={styles.tr}>
                    <td style={styles.td}>
                      <div style={styles.nameCell}>
                        <Building2 size={18} color="#8DC5AA" />
                        <span style={styles.siteName}>{site.nom}</span>
                      </div>
                    </td>
                    <td style={styles.td}>
                      <span style={styles.tag}>{site.typeSite || 'N/A'}</span>
                    </td>
                    <td style={styles.td}>{site.superficieM2?.toLocaleString()} m²</td>
                    <td style={styles.td}>
                      <span style={styles.scoreValue}>
                        {site.emissionsConstruction?.toFixed(2)} <small>tCO2e</small>
                      </span>
                    </td>
                    <td style={styles.td}>
                      <span style={{...styles.scoreValue, color: '#4caf50'}}>
                        {site.emissionsAnnuelles?.toFixed(2)} <small>tCO2e/an</small>
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            
            {filteredSites.length === 0 && (
              <div style={styles.emptyState}>
                <p>No buildings found.</p>
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
}

const styles = {
  container: {
    padding: '2rem 3rem',
    maxWidth: '1400px',
    margin: '0 auto',
    fontFamily: '"Inter", sans-serif',
    backgroundColor: '#f4f7f6',
    minHeight: '100vh',
  },
  header: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '2.5rem',
  },
  headerLeft: {
    display: 'flex',
    flexDirection: 'column',
    gap: '0.5rem',
  },
  backButton: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.4rem',
    background: 'none',
    border: 'none',
    color: '#666',
    cursor: 'pointer',
    fontSize: '0.9rem',
    padding: 0,
    width: 'fit-content',
  },
  title: {
    fontSize: '2.2rem',
    margin: 0,
    color: '#333',
    fontWeight: '800',
  },
  searchContainer: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.75rem',
    backgroundColor: 'white',
    padding: '0.75rem 1.25rem',
    borderRadius: '12px',
    border: '1px solid #eaeaea',
    width: '350px',
    boxShadow: '0 2px 4px rgba(0,0,0,0.02)',
  },
  searchInput: {
    border: 'none',
    outline: 'none',
    fontSize: '0.95rem',
    width: '100%',
    color: '#333',
  },
  content: {
    marginTop: '1rem',
  },
  tableCard: {
    backgroundColor: 'white',
    borderRadius: '20px',
    boxShadow: '0 10px 30px rgba(0,0,0,0.04)',
    border: '1px solid #eaeaea',
    overflow: 'hidden',
  },
  table: {
    width: '100%',
    borderCollapse: 'collapse',
    textAlign: 'left',
  },
  th: {
    padding: '1.25rem 1.5rem',
    backgroundColor: '#f9fbfb',
    color: '#888',
    fontWeight: '600',
    fontSize: '0.85rem',
    textTransform: 'uppercase',
    letterSpacing: '0.5px',
    borderBottom: '1px solid #f0f0f0',
  },
  tr: {
    borderBottom: '1px solid #f0f0f0',
    transition: 'background-color 0.2s',
    '&:hover': {
      backgroundColor: '#f9fbfb',
    }
  },
  td: {
    padding: '1.25rem 1.5rem',
    color: '#444',
    fontSize: '0.95rem',
  },
  nameCell: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.75rem',
  },
  siteName: {
    fontWeight: '600',
    color: '#333',
  },
  tag: {
    backgroundColor: 'rgba(141, 197, 170, 0.1)',
    color: '#8DC5AA',
    padding: '0.3rem 0.8rem',
    borderRadius: '20px',
    fontSize: '0.8rem',
    fontWeight: '600',
  },
  scoreValue: {
    fontWeight: '700',
    fontSize: '1.1rem',
    color: '#333',
  },
  centerContent: {
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    justifyContent: 'center',
    minHeight: '400px',
    gap: '1.5rem',
  },
  loader: {
    width: '40px',
    height: '40px',
    border: '4px solid rgba(141, 197, 170, 0.1)',
    borderTop: '4px solid #8DC5AA',
    borderRadius: '50%',
    animation: 'spin 1s linear infinite',
  },
  errorText: {
    color: '#ff4d4f',
    fontSize: '1.1rem',
    fontWeight: '500',
  },
  retryButton: {
    padding: '0.8rem 2rem',
    backgroundColor: '#8DC5AA',
    color: 'white',
    border: 'none',
    borderRadius: '8px',
    cursor: 'pointer',
    fontWeight: '600',
  },
  emptyState: {
    padding: '4rem',
    textAlign: 'center',
    color: '#888',
    fontSize: '1.1rem',
  }
};
