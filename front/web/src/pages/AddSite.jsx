import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { ChevronLeft, Plus, Trash2, Building2, Car, Zap, Box, Calculator, Info } from 'lucide-react';

export default function AddSite() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [showResults, setShowResults] = useState(false);

  // Catalog data
  const [catalogMaterials, setCatalogMaterials] = useState([]);
  const [catalogEnergies, setCatalogEnergies] = useState([]);

  const [formData, setFormData] = useState({
    site: {
      nom: '',
      typeSite: 'Bureaux',
      anneeConstruction: new Date().getFullYear(),
      superficieM2: 0,
      nombreEtages: 1,
      nombrePersonnes: 0,
    },
    parking: {
      nombrePlacesTotal: 0,
      placesAeriennes: 0,
      placesSousDalle: 0,
      placesSousSol: 0,
    },
    energies: [],
    materiaux: [],
  });

  const [emissions, setEmissions] = useState({
    construction: 0,
    annuelle: 0,
    detailsConstruction: {
      materiaux: 0,
      parking: 0,
    },
    detailsAnnuel: {
      electricite: 0,
      gaz: 0,
      fioul: 0,
      geothermie: 0,
      usageSurface: 0,
      employes: 0,
    }
  });

  useEffect(() => {
    fetchCatalog();
  }, []);

  const fetchCatalog = async () => {
    try {
      const token = localStorage.getItem('token');
      const [matRes, enRes] = await Promise.all([
        axios.get('http://localhost:5000/api/catalog/materiaux', { headers: { Authorization: `Bearer ${token}` } }),
        axios.get('http://localhost:5000/api/catalog/energies', { headers: { Authorization: `Bearer ${token}` } })
      ]);
      setCatalogMaterials(matRes.data);
      setCatalogEnergies(enRes.data);
    } catch (err) {
      console.error('Failed to fetch catalog', err);
    }
  };

  const handleSiteChange = (e) => {
    const { name, value } = e.target;
    let parsedValue = value;
    if (name === 'anneeConstruction' || name === 'nombreEtages' || name === 'nombrePersonnes') {
      parsedValue = parseInt(value) || 0;
    } else if (name === 'superficieM2') {
      parsedValue = parseFloat(value) || 0;
    }
    setFormData((prev) => ({
      ...prev,
      site: { ...prev.site, [name]: parsedValue },
    }));
  };

  const handleParkingChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      parking: { ...prev.parking, [name]: parseInt(value) || 0 },
    }));
  };

  const addEnergie = () => {
    const currentYear = new Date().getFullYear();
    setFormData((prev) => ({
      ...prev,
      energies: [
        ...prev.energies,
        {
          typeEnergie: catalogEnergies[0]?.typeEnergie || 'Électricité',
          consommationAnnuelle: 0,
          unite: 'kWh',
          typeDonnee: 'Réelle',
          periodeDebut: `${currentYear}-01-01T00:00:00.000Z`,
          periodeFin: `${currentYear}-12-31T23:59:59.000Z`,
        },
      ],
    }));
  };

  const removeEnergie = (index) => {
    setFormData((prev) => ({
      ...prev,
      energies: prev.energies.filter((_, i) => i !== index),
    }));
  };

  const handleEnergieChange = (index, field, value) => {
    const newEnergies = [...formData.energies];
    if (field === 'consommationAnnuelle') {
      newEnergies[index][field] = parseFloat(value) || 0;
    } else {
      newEnergies[index][field] = value;
    }
    setFormData((prev) => ({ ...prev, energies: newEnergies }));
  };

  const addMateriau = () => {
    setFormData((prev) => ({
      ...prev,
      materiaux: [...prev.materiaux, { materiauId: catalogMaterials[0]?.id || 1, quantite: 0, unite: 't' }],
    }));
  };

  const removeMateriau = (index) => {
    setFormData((prev) => ({
      ...prev,
      materiaux: prev.materiaux.filter((_, i) => i !== index),
    }));
  };

  const handleMateriauChange = (index, field, value) => {
    const newMateriaux = [...formData.materiaux];
    if (field === 'materiauId') {
      newMateriaux[index][field] = parseInt(value) || 0;
    } else if (field === 'quantite') {
      newMateriaux[index][field] = parseFloat(value) || 0;
    } else {
      newMateriaux[index][field] = value;
    }
    setFormData((prev) => ({ ...prev, materiaux: newMateriaux }));
  };

  const calculateEmissions = () => {
    // Construction
    let matEmissions = 0;
    formData.materiaux.forEach(m => {
      const factor = catalogMaterials.find(cm => cm.id === m.materiauId)?.facteurEmission || 0;
      matEmissions += (m.quantite || 0) * factor;
    });

    // Parking construction factors (dummy factors based on type)
    const parkingEmissions = (formData.parking.placesAeriennes * 0.5) + 
                             (formData.parking.placesSousDalle * 2.5) + 
                             (formData.parking.placesSousSol * 5.0);
    
    const constructionTotal = matEmissions + parkingEmissions;

    // Annual
    let enDetails = { electricite: 0, gaz: 0, fioul: 0, geothermie: 0 };
    formData.energies.forEach(e => {
      const factor = catalogEnergies.find(ce => ce.typeEnergie === e.typeEnergie)?.facteurEmission || 0;
      const val = (e.consommationAnnuelle || 0) * factor / 1000; // to tCO2e
      if (e.typeEnergie === 'Électricité') enDetails.electricite += val;
      if (e.typeEnergie === 'Gaz naturel') enDetails.gaz += val;
      if (e.typeEnergie === 'Fioul') enDetails.fioul += val;
      if (e.typeEnergie === 'Géothermie') enDetails.geothermie += val;
    });

    const usageSurface = (formData.site.superficieM2 || 0) * 0.000022; // dummy factor
    const employes = (formData.site.nombrePersonnes || 0) * 1.8 / 1000; // 0.80 tCO2e dummy scale

    const annualTotal = Object.values(enDetails).reduce((a, b) => a + b, 0) + usageSurface + employes;

    setEmissions({
      construction: constructionTotal,
      annuelle: annualTotal,
      detailsConstruction: {
        materiaux: matEmissions,
        parking: parkingEmissions,
      },
      detailsAnnuel: {
        ...enDetails,
        usageSurface,
        employes
      }
    });
    setShowResults(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!formData.site.nom) {
      setError('Site name is required.');
      return;
    }

    setLoading(true);
    setError('');

    try {
      const token = localStorage.getItem('token');
      await axios.post('http://localhost:5000/api/sites', formData, {
        headers: { Authorization: `Bearer ${token}` },
      });
      calculateEmissions();
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to create site.');
    } finally {
      setLoading(false);
    }
  };

  const buildingTypes = [
    "Bureaux",
    "Lieu de stockage",
    "Magasin",
    "Plateforme logistique",
    "Site d'usages mixtes"
  ];

  if (showResults) {
    return (
      <div style={styles.container}>
        <header style={styles.header}>
          <button onClick={() => navigate('/dashboard')} style={styles.backButton}>
            <ChevronLeft size={20} />
            Back to Dashboard
          </button>
          <h1 style={styles.title}>Emissions Calculations</h1>
        </header>

        <div style={styles.resultsGrid}>
          {/* Main Summary Cards */}
          <div style={styles.resultCard}>
            <h3 style={styles.resultCardTitle}>Émissions construction</h3>
            <p style={styles.resultCardValue}>{emissions.construction.toFixed(2)}</p>
            <span style={styles.resultCardUnit}>tCO2e</span>
          </div>
          <div style={styles.resultCard}>
            <h3 style={styles.resultCardTitle}>Émissions annuelles</h3>
            <p style={styles.resultCardValue}>{emissions.annuelle.toFixed(2)}</p>
            <span style={styles.resultCardUnit}>tCO2e/an</span>
          </div>
        </div>

        <div style={styles.detailsSection}>
          <div style={styles.detailsColumn}>
            <h4 style={styles.detailsHeading}>Détail construction</h4>
            <div style={styles.detailsRow}>
              <span>Matériaux</span>
              <span>{emissions.detailsConstruction.materiaux.toFixed(2)} tCO2e</span>
            </div>
            <div style={styles.detailsRow}>
              <span>Parking (construction)</span>
              <span>{emissions.detailsConstruction.parking.toFixed(2)} tCO2e</span>
            </div>
          </div>

          <div style={styles.detailsColumn}>
            <h4 style={styles.detailsHeading}>Détail annuel</h4>
            <div style={styles.detailsRow}>
              <span>Électricité</span>
              <span>{emissions.detailsAnnuel.electricite.toFixed(2)} tCO2e</span>
            </div>
            <div style={styles.detailsRow}>
              <span>Gaz naturel</span>
              <span>{emissions.detailsAnnuel.gaz.toFixed(2)} tCO2e</span>
            </div>
            <div style={styles.detailsRow}>
              <span>Fioul</span>
              <span>{emissions.detailsAnnuel.fioul.toFixed(2)} tCO2e</span>
            </div>
            <div style={styles.detailsRow}>
              <span>Géothermie</span>
              <span>{emissions.detailsAnnuel.geothermie.toFixed(2)} tCO2e</span>
            </div>
            <div style={styles.detailsRow}>
              <span>Usage surface</span>
              <span>{emissions.detailsAnnuel.usageSurface.toFixed(2)} tCO2e</span>
            </div>
            <div style={styles.detailsRow}>
              <span>Employés (déplacements)</span>
              <span>{emissions.detailsAnnuel.employes.toFixed(2)} tCO2e</span>
            </div>
          </div>
        </div>

        <button onClick={() => setShowResults(false)} style={styles.secondaryButton}>
          Back to Edit
        </button>
      </div>
    );
  }

  return (
    <div style={styles.container}>
      <header style={styles.header}>
        <button onClick={() => navigate('/dashboard')} style={styles.backButton}>
          <ChevronLeft size={20} />
          Back to Dashboard
        </button>
        <h1 style={styles.title}>Add New Site</h1>
      </header>

      <form onSubmit={handleSubmit} style={styles.form}>
        {error && <div style={styles.error}>{error}</div>}

        {/* Site Section */}
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
                value={formData.site.nom}
                onChange={handleSiteChange}
                style={styles.input}
                required
              />
            </div>
            <div style={styles.inputGroup}>
              <label style={styles.label}>Type</label>
              <select
                name="typeSite"
                value={formData.site.typeSite}
                onChange={handleSiteChange}
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
                value={formData.site.anneeConstruction}
                onChange={handleSiteChange}
                style={styles.input}
              />
            </div>
            <div style={styles.inputGroup}>
              <label style={styles.label}>Surface Area (m²)</label>
              <input
                type="number"
                name="superficieM2"
                value={formData.site.superficieM2}
                onChange={handleSiteChange}
                style={styles.input}
              />
            </div>
            <div style={styles.inputGroup}>
              <label style={styles.label}>Nombre d'étages</label>
              <input
                type="number"
                name="nombreEtages"
                value={formData.site.nombreEtages}
                onChange={handleSiteChange}
                style={styles.input}
              />
            </div>
            <div style={styles.inputGroup}>
              <label style={styles.label}>Number of Persons</label>
              <input
                type="number"
                name="nombrePersonnes"
                value={formData.site.nombrePersonnes}
                onChange={handleSiteChange}
                style={styles.input}
              />
            </div>
          </div>
        </section>

        {/* Parking Section */}
        <section style={styles.section}>
          <div style={styles.sectionHeader}>
            <Car size={24} color="#8DC5AA" />
            <h2 style={styles.sectionTitle}>Parking Details</h2>
          </div>
          <div style={styles.grid}>
            <div style={styles.inputGroup}>
              <label style={styles.label}>Total Places</label>
              <input
                type="number"
                name="nombrePlacesTotal"
                value={formData.parking.nombrePlacesTotal}
                onChange={handleParkingChange}
                style={styles.input}
              />
            </div>
            <div style={styles.inputGroup}>
              <label style={styles.label}>Aerial Places</label>
              <input
                type="number"
                name="placesAeriennes"
                value={formData.parking.placesAeriennes}
                onChange={handleParkingChange}
                style={styles.input}
              />
            </div>
            <div style={styles.inputGroup}>
              <label style={styles.label}>Under Slab Places</label>
              <input
                type="number"
                name="placesSousDalle"
                value={formData.parking.placesSousDalle}
                onChange={handleParkingChange}
                style={styles.input}
              />
            </div>
            <div style={styles.inputGroup}>
              <label style={styles.label}>Underground Places</label>
              <input
                type="number"
                name="placesSousSol"
                value={formData.parking.placesSousSol}
                onChange={handleParkingChange}
                style={styles.input}
              />
            </div>
          </div>
        </section>

        {/* Energies Section */}
        <section style={styles.section}>
          <div style={styles.sectionHeader}>
            <Zap size={24} color="#8DC5AA" />
            <h2 style={styles.sectionTitle}>Energy Consumption</h2>
            <button type="button" onClick={addEnergie} style={styles.addButton}>
              <Plus size={16} /> Add
            </button>
          </div>
          {formData.energies.map((en, index) => (
            <div key={index} style={styles.dynamicRow}>
              <select
                value={en.typeEnergie}
                onChange={(e) => handleEnergieChange(index, 'typeEnergie', e.target.value)}
                style={{...styles.input, flex: 2}}
              >
                {catalogEnergies.map(ce => <option key={ce.id} value={ce.typeEnergie}>{ce.typeEnergie}</option>)}
              </select>
              <input
                type="number"
                placeholder="Value (kWh)"
                value={en.consommationAnnuelle}
                onChange={(e) => handleEnergieChange(index, 'consommationAnnuelle', parseFloat(e.target.value))}
                style={{...styles.input, flex: 1}}
              />
              <button type="button" onClick={() => removeEnergie(index)} style={styles.deleteButton}>
                <Trash2 size={18} />
              </button>
            </div>
          ))}
          {formData.energies.length === 0 && <p style={styles.emptyNote}>No energy data added. Click "Add" to start.</p>}
        </section>

        {/* Materials Section */}
        <section style={styles.section}>
          <div style={styles.sectionHeader}>
            <Box size={24} color="#8DC5AA" />
            <h2 style={styles.sectionTitle}>Materials</h2>
            <button type="button" onClick={addMateriau} style={styles.addButton}>
              <Plus size={16} /> Add
            </button>
          </div>
          {formData.materiaux.map((mat, index) => (
            <div key={index} style={styles.dynamicRow}>
              <select
                value={mat.materiauId}
                onChange={(e) => handleMateriauChange(index, 'materiauId', parseInt(e.target.value))}
                style={{...styles.input, flex: 2}}
              >
                {catalogMaterials.map(cm => <option key={cm.id} value={cm.id}>{cm.nom}</option>)}
              </select>
              <input
                type="number"
                placeholder="Quantity (t)"
                value={mat.quantite}
                onChange={(e) => handleMateriauChange(index, 'quantite', parseFloat(e.target.value))}
                style={{...styles.input, flex: 1}}
              />
              <button type="button" onClick={() => removeMateriau(index)} style={styles.deleteButton}>
                <Trash2 size={18} />
              </button>
            </div>
          ))}
          {formData.materiaux.length === 0 && <p style={styles.emptyNote}>No materials added. Click "Add" to start.</p>}
        </section>

        <div style={styles.actions}>
          <button type="submit" style={styles.submitButton} disabled={loading}>
            {loading ? 'Creating Site...' : 'Create and Calculate'}
          </button>
        </div>
      </form>
    </div>
  );
}

const styles = {
  container: {
    padding: '2rem 3rem',
    maxWidth: '1200px',
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
  },
  title: {
    fontSize: '2rem',
    margin: 0,
    color: '#333',
    fontWeight: '700',
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
    '&:focus': {
      borderColor: '#8DC5AA',
      backgroundColor: 'white',
      boxShadow: '0 0 0 3px rgba(141, 197, 170, 0.2)',
    }
  },
  dynamicRow: {
    display: 'flex',
    gap: '1.5rem',
    marginBottom: '1rem',
    alignItems: 'center',
  },
  addButton: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.5rem',
    padding: '0.6rem 1.2rem',
    backgroundColor: '#8DC5AA',
    color: 'white',
    border: 'none',
    borderRadius: '8px',
    cursor: 'pointer',
    fontSize: '0.9rem',
    fontWeight: '600',
    transition: 'transform 0.1s',
  },
  deleteButton: {
    background: 'none',
    border: 'none',
    color: '#ff4d4f',
    cursor: 'pointer',
    padding: '0.5rem',
    borderRadius: '50%',
    transition: 'background 0.2s',
    '&:hover': {
      backgroundColor: '#fff1f0',
    }
  },
  emptyNote: {
    color: '#999',
    fontSize: '0.9rem',
    fontStyle: 'italic',
    textAlign: 'center',
    marginTop: '1rem',
  },
  actions: {
    display: 'flex',
    justifyContent: 'flex-end',
    marginTop: '1rem',
    paddingBottom: '3rem',
  },
  submitButton: {
    padding: '1.2rem 3rem',
    backgroundColor: '#8DC5AA',
    color: 'white',
    border: 'none',
    borderRadius: '12px',
    fontSize: '1.1rem',
    fontWeight: 'bold',
    cursor: 'pointer',
    boxShadow: '0 4px 15px rgba(141, 197, 170, 0.3)',
    transition: 'all 0.2s',
  },
  secondaryButton: {
    padding: '1rem 2rem',
    backgroundColor: 'white',
    color: '#666',
    border: '1px solid #ddd',
    borderRadius: '10px',
    fontSize: '1rem',
    fontWeight: '600',
    cursor: 'pointer',
    marginTop: '2rem',
  },

  // Results Styles
  resultsGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))',
    gap: '2rem',
    marginBottom: '3rem',
  },
  resultCard: {
    backgroundColor: 'white',
    padding: '2.5rem',
    borderRadius: '20px',
    textAlign: 'center',
    boxShadow: '0 10px 30px rgba(0,0,0,0.05)',
    border: '1px solid #eaeaea',
  },
  resultCardTitle: {
    fontSize: '1.1rem',
    color: '#888',
    marginBottom: '1rem',
    fontWeight: '500',
  },
  resultCardValue: {
    fontSize: '3.5rem',
    color: '#8DC5AA',
    fontWeight: '800',
    margin: 0,
    lineHeight: 1,
  },
  resultCardUnit: {
    fontSize: '1.2rem',
    color: '#555',
    fontWeight: '600',
  },
  detailsSection: {
    display: 'flex',
    gap: '3rem',
    flexWrap: 'wrap',
  },
  detailsColumn: {
    flex: 1,
    minWidth: '350px',
    backgroundColor: 'white',
    padding: '2rem',
    borderRadius: '16px',
    border: '1px solid #eaeaea',
  },
  detailsHeading: {
    fontSize: '1.25rem',
    color: '#333',
    marginBottom: '1.5rem',
    paddingBottom: '0.75rem',
    borderBottom: '2px solid #f4f7f6',
  },
  detailsRow: {
    display: 'flex',
    justifyContent: 'space-between',
    padding: '1rem 0',
    borderBottom: '1px solid #f9f9f9',
    fontSize: '1rem',
    color: '#555',
    '&:last-child': {
      borderBottom: 'none',
    }
  }
};
