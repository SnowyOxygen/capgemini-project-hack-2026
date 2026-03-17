import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Building2 } from 'lucide-react';
import axios from 'axios';
import { calculateTotalCO2 } from '../components/CO2EmissionsChart';
import { calculateTotalEnergy } from '../components/EnergyConsumptionChart';

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

  const handleSiteClick = (siteId) => {
    navigate(`/site/${siteId}`);
  };

  return (
    <div style={styles.content}>
      {/* Header */}
      <div style={styles.header}>
        <h1 style={styles.pageTitle}>All Sites</h1>
        <p style={styles.pageSubtitle}>Click on a site to view detailed analytics</p>
      </div>

      {/* KPI Card */}
      <div style={styles.kpiCard}>
        <div style={styles.kpiIconWrapper}>
          <Building2 size={24} color="#8DC5AA" />
        </div>
        <div style={styles.kpiInfo}>
          <h3 style={styles.kpiTitle}>Total Sites</h3>
          <p style={styles.kpiValue}>
            {loading ? 'Loading...' : error ? 'Error loading data' : sites.length}
          </p>
        </div>
      </div>

      {/* Sites List */}
      {loading ? (
        <div style={styles.loadingMessage}>Loading sites...</div>
      ) : error ? (
        <div style={styles.errorMessage}>{error}</div>
      ) : sites.length === 0 ? (
        <div style={styles.emptyMessage}>No sites available. Click "Add Site" to create one.</div>
      ) : (
        <div style={styles.sitesGrid}>
          {sites.map(site => {
            const totalCO2 = calculateTotalCO2(site.materiaux);
            const totalEnergy = calculateTotalEnergy(site.energies);

            return (
              <div 
                key={site.id} 
                className="site-card-clickable"
                style={styles.siteCard}
                onClick={() => handleSiteClick(site.id)}
              >
                <div style={styles.siteCardHeader}>
                  <h3 style={styles.siteCardName}>{site.nom}</h3>
                  <span style={styles.siteCardType}>{site.typeSite || 'N/A'}</span>
                </div>
                
                <div style={styles.siteCardDetails}>
                  <div style={styles.detailRow}>
                    <span style={styles.detailLabel}>Surface:</span>
                    <span style={styles.detailValue}>
                      {site.superficieM2 ? `${site.superficieM2.toFixed(0)} m²` : 'N/A'}
                    </span>
                  </div>
                  <div style={styles.detailRow}>
                    <span style={styles.detailLabel}>Year:</span>
                    <span style={styles.detailValue}>{site.anneeConstruction || 'N/A'}</span>
                  </div>
                  <div style={styles.detailRow}>
                    <span style={styles.detailLabel}>CO2:</span>
                    <span style={styles.detailValue}>{totalCO2.toFixed(2)} tons</span>
                  </div>
                  <div style={styles.detailRow}>
                    <span style={styles.detailLabel}>Energy:</span>
                    <span style={styles.detailValue}>
                      {totalEnergy > 0 ? `${totalEnergy.toLocaleString()} kWh` : 'N/A'}
                    </span>
                  </div>
                </div>

                <div style={styles.viewDetailsButton}>
                  View Details →
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}

const styles = {
  content: {
    padding: '3rem',
    flex: 1,
    fontFamily: '"Inter", sans-serif',
    backgroundColor: '#f4f7f6',
    minHeight: '100vh',
  },
  header: {
    marginBottom: '2rem',
  },
  pageTitle: {
    margin: '0 0 0.5rem 0',
    fontSize: '2rem',
    fontWeight: 'bold',
    color: '#333',
  },
  pageSubtitle: {
    margin: 0,
    fontSize: '1rem',
    color: '#666',
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
    gridTemplateColumns: 'repeat(auto-fill, minmax(350px, 1fr))',
    gap: '1.5rem',
    marginTop: '1rem',
  },
  siteCard: {
    backgroundColor: 'white',
    borderRadius: '16px',
    padding: '1.5rem',
    boxShadow: '0 4px 6px rgba(0,0,0,0.05)',
    border: '1px solid #eaeaea',
    transition: 'transform 0.2s, box-shadow 0.2s',
    cursor: 'pointer',
  },
  siteCardHeader: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '1rem',
    paddingBottom: '1rem',
    borderBottom: '1px solid #eaeaea',
  },
  siteCardName: {
    margin: 0,
    fontSize: '1.25rem',
    fontWeight: 'bold',
    color: '#333',
  },
  siteCardType: {
    padding: '0.25rem 0.75rem',
    backgroundColor: '#8DC5AA',
    color: 'white',
    borderRadius: '12px',
    fontSize: '0.85rem',
    fontWeight: '500',
  },
  siteCardDetails: {
    display: 'flex',
    flexDirection: 'column',
    gap: '0.5rem',
    marginBottom: '1rem',
  },
  detailRow: {
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
  viewDetailsButton: {
    textAlign: 'center',
    padding: '0.75rem',
    backgroundColor: '#8DC5AA',
    color: 'white',
    borderRadius: '8px',
    fontSize: '0.9rem',
    fontWeight: '600',
    transition: 'background 0.2s',
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
