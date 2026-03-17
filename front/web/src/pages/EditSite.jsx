import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import axios from 'axios';
import { ChevronLeft, Building2, Save } from 'lucide-react';

export default function EditSite() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');

  const [formData, setFormData] = useState({
    nom: '',
    typeSite: 'Bureaux',
    anneeConstruction: new Date().getFullYear(),
    superficieM2: 0,
    nombreEtages: 1,
    nombrePersonnes: 0,
  });

  const buildingTypes = [
    "Bureaux",
    "Lieu de stockage",
    "Magasin",
    "Plateforme logistique",
    "Site d'usages mixtes"
  ];

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
      
      const site = response.data.find(s => s.id === parseInt(id));
      if (site) {
        setFormData({
          nom: site.nom || '',
          typeSite: site.typeSite || 'Bureaux',
          anneeConstruction: site.anneeConstruction || new Date().getFullYear(),
          superficieM2: site.superficieM2 || 0,
          nombreEtages: site.nombreEtages || 1,
          nombrePersonnes: site.nombrePersonnes || 0,
        });
      } else {
        setError('Site not found');
      }
      setLoading(false);
    } catch (err) {
      console.error('Error fetching site:', err);
      setError('Failed to load site data');
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    let parsedValue = value;
    
    if (name === 'anneeConstruction' || name === 'nombreEtages' || name === 'nombrePersonnes') {
      parsedValue = parseInt(value) || 0;
    } else if (name === 'superficieM2') {
      parsedValue = parseFloat(value) || 0;
    }
    
    setFormData((prev) => ({
      ...prev,
      [name]: parsedValue,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!formData.nom) {
      setError('Site name is required.');
      return;
    }

    setSaving(true);
    setError('');

    try {
      const token = localStorage.getItem('token');
      await axios.patch(
        `${import.meta.env.VITE_API_URL}/api/Sites/${id}`, 
        formData,
        {
          headers: { 
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
          },
        }
      );
      
      // Navigate back to site detail page
      navigate(`/site/${id}`);
    } catch (err) {
      console.error('Error updating site:', err);
      setError(err.response?.data?.message || 'Failed to update site.');
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div style={styles.container}>
        <div style={styles.loadingMessage}>Loading site data...</div>
      </div>
    );
  }

  return (
    <div style={styles.container}>
      <header style={styles.header}>
        <button onClick={() => navigate(`/site/${id}`)} style={styles.backButton}>
          <ChevronLeft size={20} />
          Back to Site Details
        </button>
        <h1 style={styles.title}>Edit Site</h1>
        <p style={styles.subtitle}>Update basic site information</p>
      </header>

      <form onSubmit={handleSubmit} style={styles.form}>
        {error && <div style={styles.error}>{error}</div>}

        {/* Basic Information Section */}
        <section style={styles.section}>
          <div style={styles.sectionHeader}>
            <Building2 size={24} color="#8DC5AA" />
            <h2 style={styles.sectionTitle}>Basic Information</h2>
          </div>
          
          <div style={styles.grid}>
            <div style={styles.inputGroup}>
              <label style={styles.label}>Site Name *</label>
              <input
                type="text"
                name="nom"
                value={formData.nom}
                onChange={handleChange}
                style={styles.input}
                required
              />
            </div>

            <div style={styles.inputGroup}>
              <label style={styles.label}>Type</label>
              <select
                name="typeSite"
                value={formData.typeSite}
                onChange={handleChange}
                style={styles.input}
              >
                {buildingTypes.map(t => <option key={t} value={t}>{t}</option>)}
              </select>
            </div>

            <div style={styles.inputGroup}>
              <label style={styles.label}>Year of Construction</label>
              <input
                type="number"
                name="anneeConstruction"
                value={formData.anneeConstruction}
                onChange={handleChange}
                style={styles.input}
                min="1800"
                max="2100"
              />
            </div>

            <div style={styles.inputGroup}>
              <label style={styles.label}>Surface Area (m²)</label>
              <input
                type="number"
                name="superficieM2"
                value={formData.superficieM2}
                onChange={handleChange}
                style={styles.input}
                min="0"
                step="0.01"
              />
            </div>

            <div style={styles.inputGroup}>
              <label style={styles.label}>Nombre d'étages</label>
              <input
                type="number"
                name="nombreEtages"
                value={formData.nombreEtages}
                onChange={handleChange}
                style={styles.input}
                min="1"
                max="1000"
              />
            </div>

            <div style={styles.inputGroup}>
              <label style={styles.label}>Number of Persons</label>
              <input
                type="number"
                name="nombrePersonnes"
                value={formData.nombrePersonnes}
                onChange={handleChange}
                style={styles.input}
                min="0"
                max="1000000"
              />
            </div>
          </div>
        </section>

        <div style={styles.infoBox}>
          <Building2 size={20} color="#666" />
          <p style={styles.infoText}>
            This form only updates basic site information. To modify parking, energy consumption, 
            or materials data, use the dedicated endpoints on the site detail page.
          </p>
        </div>

        <div style={styles.actions}>
          <button 
            type="button" 
            onClick={() => navigate(`/site/${id}`)} 
            style={styles.cancelButton}
            disabled={saving}
          >
            Cancel
          </button>
          <button 
            type="submit" 
            style={styles.submitButton} 
            disabled={saving}
          >
            <Save size={18} />
            {saving ? 'Saving...' : 'Save Changes'}
          </button>
        </div>
      </form>
    </div>
  );
}

const styles = {
  container: {
    padding: '2rem 3rem',
    maxWidth: '1000px',
    margin: '0 auto',
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
    background: 'none',
    border: 'none',
    color: '#666',
    cursor: 'pointer',
    fontSize: '0.9rem',
    marginBottom: '1rem',
    padding: 0,
    transition: 'color 0.2s',
  },
  title: {
    fontSize: '2rem',
    margin: '0 0 0.5rem 0',
    color: '#333',
    fontWeight: '700',
  },
  subtitle: {
    fontSize: '1rem',
    margin: 0,
    color: '#666',
  },
  form: {
    display: 'flex',
    flexDirection: 'column',
    gap: '2rem',
  },
  error: {
    backgroundColor: '#ffebee',
    color: '#c62828',
    padding: '1rem',
    borderRadius: '8px',
    fontSize: '0.9rem',
    border: '1px solid #ffcdd2',
  },
  section: {
    backgroundColor: 'white',
    padding: '2rem',
    borderRadius: '16px',
    boxShadow: '0 4px 6px rgba(0,0,0,0.05)',
    border: '1px solid #eaeaea',
  },
  sectionHeader: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.75rem',
    marginBottom: '2rem',
    borderBottom: '1px solid #f0f0f0',
    paddingBottom: '1rem',
  },
  sectionTitle: {
    margin: 0,
    fontSize: '1.4rem',
    color: '#333',
    fontWeight: '600',
    flex: 1,
  },
  grid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))',
    gap: '2rem',
  },
  inputGroup: {
    display: 'flex',
    flexDirection: 'column',
    gap: '0.75rem',
  },
  label: {
    fontSize: '0.95rem',
    color: '#555',
    fontWeight: '500',
  },
  input: {
    padding: '0.85rem',
    borderRadius: '10px',
    border: '1px solid #ddd',
    fontSize: '1rem',
    outline: 'none',
    transition: 'all 0.2s',
    backgroundColor: '#f9f9f9',
  },
  infoBox: {
    display: 'flex',
    alignItems: 'flex-start',
    gap: '1rem',
    padding: '1.5rem',
    backgroundColor: '#E8F5E9',
    borderRadius: '12px',
    border: '1px solid #C8E6C9',
  },
  infoText: {
    margin: 0,
    fontSize: '0.9rem',
    color: '#2E7D32',
    lineHeight: '1.5',
  },
  actions: {
    display: 'flex',
    gap: '1rem',
    justifyContent: 'flex-end',
    paddingTop: '1rem',
  },
  cancelButton: {
    padding: '0.85rem 2rem',
    backgroundColor: 'white',
    color: '#666',
    border: '2px solid #ddd',
    borderRadius: '10px',
    cursor: 'pointer',
    fontSize: '1rem',
    fontWeight: '600',
    transition: 'all 0.2s',
  },
  submitButton: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.5rem',
    padding: '0.85rem 2rem',
    backgroundColor: '#8DC5AA',
    color: 'white',
    border: 'none',
    borderRadius: '10px',
    cursor: 'pointer',
    fontSize: '1rem',
    fontWeight: '600',
    transition: 'all 0.2s',
  },
  loadingMessage: {
    textAlign: 'center',
    padding: '3rem',
    fontSize: '1.125rem',
    color: '#666',
  },
};
