import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Trash2, Building2, ChevronLeft, Edit } from 'lucide-react';
import axios from 'axios';
import CO2EmissionsChart, { calculateTotalCO2 } from '../components/CO2EmissionsChart';
import EnergyConsumptionChart, { calculateTotalEnergy } from '../components/EnergyConsumptionChart';
import ParkingDistributionChart, { calculateTotalParking } from '../components/ParkingDistributionChart';
import BuildingEfficiencyGauge, { calculateCO2PerM2, calculateEnergyPerPerson } from '../components/BuildingEfficiencyGauge';
import MaterialQuantityChart, { calculateTotalMaterialWeight } from '../components/MaterialQuantityChart';

export default function SiteDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [site, setSite] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetchSite();
  }, [id]);

  const fetchSite = async () => {
    try {
      const token = localStorage.getItem('token');
      const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/Sites`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      const foundSite = response.data.find(s => s.id === parseInt(id));
      if (foundSite) {
        setSite(foundSite);
      } else {
        setError('Site not found');
      }
      setLoading(false);
    } catch (err) {
      console.error('Error fetching site:', err);
      setError('Failed to load site');
      setLoading(false);
    }
  };

  const handleDeleteSite = async () => {
    if (!window.confirm(`Are you sure you want to delete "${site.nom}"? This action cannot be undone.`)) {
      return;
    }

    try {
      const token = localStorage.getItem('token');
      await axios.delete(`${import.meta.env.VITE_API_URL}/api/Sites/${id}`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      
      navigate('/dashboard');
    } catch (err) {
      console.error('Error deleting site:', err);
      alert('Failed to delete site. Please try again.');
    }
  };

  if (loading) {
    return <div style={styles.loadingMessage}>Loading site...</div>;
  }

  if (error || !site) {
    return <div style={styles.errorMessage}>{error || 'Site not found'}</div>;
  }

  const totalCO2 = calculateTotalCO2(site.materiaux);
  const totalEnergy = calculateTotalEnergy(site.energies);
  const co2PerM2 = calculateCO2PerM2(totalCO2, site.superficieM2);
  const energyPerPerson = calculateEnergyPerPerson(totalEnergy, site.nombrePersonnes);
  const totalParking = calculateTotalParking(site.parking);
  const totalMaterialWeight = calculateTotalMaterialWeight(site.materiaux);

  return (
    <div style={styles.content}>
      {/* Header with back button */}
      <div style={styles.header}>
        <button onClick={() => navigate('/dashboard')} style={styles.backButton}>
          <ChevronLeft size={20} />
          Back to Site List
        </button>
      </div>

      {/* Site Card */}
      <div style={styles.siteCard}>
        <div style={styles.siteHeader}>
          <h3 style={styles.siteName}>{site.nom}</h3>
          <div style={styles.siteHeaderRight}>
            <span style={styles.siteType}>{site.typeSite || 'N/A'}</span>
            <button 
              style={styles.editButton}
              onClick={() => navigate(`/site/${id}/edit`)}
              title="Edit site"
            >
              <Edit size={18} />
            </button>
            <button 
              className="site-delete-button"
              style={styles.deleteButton}
              onClick={handleDeleteSite}
              title="Delete site"
            >
              <Trash2 size={18} />
            </button>
          </div>
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
            <span style={styles.detailLabel}>Occupants:</span>
            <span style={styles.detailValue}>{site.nombrePersonnes || 'N/A'}</span>
          </div>
          <div style={styles.detailItem}>
            <span style={styles.detailLabel}>Total CO2:</span>
            <span style={styles.detailValue}>{totalCO2.toFixed(2)} tons</span>
          </div>
          <div style={styles.detailItem}>
            <span style={styles.detailLabel}>Total Energy:</span>
            <span style={styles.detailValue}>{totalEnergy > 0 ? `${totalEnergy.toLocaleString()} kWh` : 'N/A'}</span>
          </div>
          <div style={styles.detailItem}>
            <span style={styles.detailLabel}>Parking Spaces:</span>
            <span style={styles.detailValue}>{totalParking || 'N/A'}</span>
          </div>
          <div style={styles.detailItem}>
            <span style={styles.detailLabel}>Material Weight:</span>
            <span style={styles.detailValue}>{totalMaterialWeight ? `${totalMaterialWeight.toFixed(0)} t` : 'N/A'}</span>
          </div>
        </div>

        {/* Efficiency Gauges */}
        <div style={styles.gaugesContainer}>
          <BuildingEfficiencyGauge 
            value={co2PerM2} 
            metric="CO2/m²"
            unit="kg/m²"
            title="CO2 Efficiency"
            height={220}
          />
          <BuildingEfficiencyGauge 
            value={energyPerPerson} 
            metric="Energy/Person"
            unit="kWh/person"
            title="Energy Efficiency"
            height={220}
            max={energyPerPerson ? energyPerPerson * 2 : 5000}
          />
        </div>

        {/* Main Charts Grid */}
        <div style={styles.chartsContainer}>
          <CO2EmissionsChart materiaux={site.materiaux} />
          <EnergyConsumptionChart energies={site.energies} />
        </div>

        {/* Secondary Charts Grid */}
        <div style={styles.chartsContainer}>
          <ParkingDistributionChart parking={site.parking} />
          <MaterialQuantityChart materiaux={site.materiaux} />
        </div>
      </div>
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
  backButton: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.5rem',
    padding: '0.75rem 1rem',
    backgroundColor: 'white',
    border: '1px solid #eaeaea',
    borderRadius: '8px',
    fontSize: '1rem',
    color: '#333',
    cursor: 'pointer',
    transition: 'all 0.2s',
    fontWeight: '500',
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
  siteCard: {
    backgroundColor: 'white',
    borderRadius: '16px',
    padding: '1.5rem',
    boxShadow: '0 4px 6px rgba(0,0,0,0.05)',
    border: '1px solid #eaeaea',
  },
  siteHeader: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '1rem',
    paddingBottom: '1rem',
    borderBottom: '1px solid #eaeaea',
  },  headerButtons: {
    display: 'flex',
    gap: '0.75rem',
  },  siteName: {
    margin: 0,
    fontSize: '1.5rem',
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
  siteHeaderRight: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.75rem',
  },
  editButton: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    padding: '0.5rem',
    backgroundColor: '#8DC5AA',
    color: 'white',
    border: '1px solid #8DC5AA',
    borderRadius: '8px',
    cursor: 'pointer',
    transition: 'all 0.2s',
    fontSize: '0.9rem',
  },
  deleteButton: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    padding: '0.5rem',
    backgroundColor: 'transparent',
    color: '#F44336',
    border: '1px solid #F44336',
    borderRadius: '8px',
    cursor: 'pointer',
    transition: 'all 0.2s',
    fontSize: '0.9rem',
  },
  siteDetails: {
    display: 'grid',
    gridTemplateColumns: 'repeat(4, 1fr)',
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
  gaugesContainer: {
    display: 'grid',
    gridTemplateColumns: '1fr 1fr',
    gap: '1rem',
    marginTop: '1rem',
    marginBottom: '1rem',
  },
  chartsContainer: {
    display: 'grid',
    gridTemplateColumns: '1fr 1fr',
    gap: '1rem',
    marginTop: '1rem',
  },
};
