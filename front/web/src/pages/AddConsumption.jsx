import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { ChevronLeft, Plus, Trash2, Zap, Save, List, BarChart3 } from 'lucide-react';
import axios from 'axios';
import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';

const HighchartsReactComponent = HighchartsReact.default || HighchartsReact;

export default function AddConsumption() {
  const navigate = useNavigate();
  const [sites, setSites] = useState([]);
  const [selectedSiteId, setSelectedSiteId] = useState('');
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [activeTab, setActiveTab] = useState('add'); // 'add', 'view', 'charts'
  const [allConsumptionData, setAllConsumptionData] = useState([]);

  const energyTypes = ['Électricité', 'Gaz naturel', 'Fioul', 'Géothermie', 'Autre'];
  const dataTypes = ['Réelle', 'Estimée', 'Calculée'];

  const [consumptionRecords, setConsumptionRecords] = useState([
    {
      typeEnergie: 'Électricité',
      consommationAnnuelle: 0,
      unite: 'kWh',
      typeDonnee: 'Réelle',
      periodeDebut: `${new Date().getFullYear()}-01-01T00:00:00.000Z`,
      periodeFin: `${new Date().getFullYear()}-12-31T23:59:59.000Z`,
    }
  ]);

  useEffect(() => {
    fetchSites();
    fetchAllConsumptionData();
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
      if (response.data.length > 0) {
        setSelectedSiteId(response.data[0].id.toString());
      }
      setLoading(false);
    } catch (err) {
      console.error('Error fetching sites:', err);
      setError('Failed to load sites');
      setLoading(false);
    }
  };

  const fetchAllConsumptionData = async () => {
    try {
      const token = localStorage.getItem('token');
      const response = await axios.get(`${import.meta.env.VITE_API_URL}/api/Sites`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      
      // Extract all consumption records with site information
      const allRecords = [];
      response.data.forEach(site => {
        if (site.energies && site.energies.length > 0) {
          site.energies.forEach(energy => {
            allRecords.push({
              ...energy,
              siteName: site.nom,
              siteId: site.id,
              siteType: site.typeSite
            });
          });
        }
      });
      setAllConsumptionData(allRecords);
    } catch (err) {
      console.error('Error fetching consumption data:', err);
    }
  };

  const addRecord = () => {
    const currentYear = new Date().getFullYear();
    setConsumptionRecords([
      ...consumptionRecords,
      {
        typeEnergie: 'Électricité',
        consommationAnnuelle: 0,
        unite: 'kWh',
        typeDonnee: 'Réelle',
        periodeDebut: `${currentYear}-01-01T00:00:00.000Z`,
        periodeFin: `${currentYear}-12-31T23:59:59.000Z`,
      }
    ]);
  };

  const removeRecord = (index) => {
    setConsumptionRecords(consumptionRecords.filter((_, i) => i !== index));
  };

  const updateRecord = (index, field, value) => {
    const newRecords = [...consumptionRecords];
    if (field === 'consommationAnnuelle') {
      newRecords[index][field] = parseFloat(value) || 0;
    } else {
      newRecords[index][field] = value;
    }
    setConsumptionRecords(newRecords);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!selectedSiteId) {
      setError('Please select a site.');
      return;
    }

    if (consumptionRecords.length === 0) {
      setError('Please add at least one consumption record.');
      return;
    }

    // Validate all records
    for (let i = 0; i < consumptionRecords.length; i++) {
      const record = consumptionRecords[i];
      if (!record.consommationAnnuelle || record.consommationAnnuelle <= 0) {
        setError(`Record ${i + 1}: Consumption amount must be greater than 0.`);
        return;
      }
    }

    setSaving(true);
    setError('');
    setSuccess('');

    try {
      const token = localStorage.getItem('token');
      
      // Submit each record individually
      const promises = consumptionRecords.map(record =>
        axios.post(
          `${import.meta.env.VITE_API_URL}/api/Sites/${selectedSiteId}/energies`,
          record,
          {
            headers: {
              'Authorization': `Bearer ${token}`,
              'Content-Type': 'application/json'
            }
          }
        )
      );

      await Promise.all(promises);
      
      setSuccess(`Successfully added ${consumptionRecords.length} consumption record(s)!`);
      
      // Refresh consumption data
      fetchAllConsumptionData();
      
      // Reset form after 2 seconds
      setTimeout(() => {
        setConsumptionRecords([
          {
            typeEnergie: 'Électricité',
            consommationAnnuelle: 0,
            unite: 'kWh',
            typeDonnee: 'Réelle',
            periodeDebut: `${new Date().getFullYear()}-01-01T00:00:00.000Z`,
            periodeFin: `${new Date().getFullYear()}-12-31T23:59:59.000Z`,
          }
        ]);
        setSuccess('');
      }, 2000);
      
    } catch (err) {
      console.error('Error adding consumption data:', err);
      setError(err.response?.data?.message || 'Failed to add consumption data. Please try again.');
    } finally {
      setSaving(false);
    }
  };

  // Generate chart for consumption by energy type
  const getConsumptionByTypeChart = () => {
    const typeData = {};
    allConsumptionData.forEach(record => {
      const type = record.typeEnergie || 'Unknown';
      typeData[type] = (typeData[type] || 0) + (record.consommationAnnuelle || 0);
    });

    return {
      chart: { type: 'pie' },
      title: { text: 'Total Consumption by Energy Type' },
      tooltip: {
        pointFormat: '<b>{point.y:.2f} kWh</b> ({point.percentage:.1f}%)'
      },
      series: [{
        name: 'Consumption',
        colorByPoint: true,
        data: Object.entries(typeData).map(([name, value]) => ({
          name,
          y: value
        }))
      }]
    };
  };

  // Generate chart for consumption by site
  const getConsumptionBySiteChart = () => {
    const siteData = {};
    allConsumptionData.forEach(record => {
      const site = record.siteName || 'Unknown';
      siteData[site] = (siteData[site] || 0) + (record.consommationAnnuelle || 0);
    });

    return {
      chart: { type: 'column' },
      title: { text: 'Total Consumption by Site' },
      xAxis: {
        categories: Object.keys(siteData),
        title: { text: 'Sites' }
      },
      yAxis: {
        title: { text: 'Consumption (kWh)' }
      },
      tooltip: {
        valueSuffix: ' kWh'
      },
      series: [{
        name: 'Consumption',
        data: Object.values(siteData),
        color: '#8DC5AA'
      }]
    };
  };

  // Generate chart for data type distribution
  const getDataTypeDistributionChart = () => {
    const typeCount = {};
    allConsumptionData.forEach(record => {
      const type = record.typeDonnee || 'Unknown';
      typeCount[type] = (typeCount[type] || 0) + 1;
    });

    return {
      chart: { type: 'bar' },
      title: { text: 'Data Type Distribution' },
      xAxis: {
        categories: Object.keys(typeCount),
        title: { text: null }
      },
      yAxis: {
        title: { text: 'Number of Records' }
      },
      series: [{
        name: 'Records',
        data: Object.values(typeCount),
        colorByPoint: true
      }],
      legend: { enabled: false }
    };
  };

  if (loading) {
    return (
      <div style={styles.container}>
        <div style={styles.loadingMessage}>Loading...</div>
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
        <h1 style={styles.title}>Energy Consumption Management</h1>
        <p style={styles.subtitle}>Add, view, and analyze consumption data</p>
      </header>

      {/* Tabs */}
      <div style={styles.tabs}>
        <button
          onClick={() => setActiveTab('add')}
          style={{
            ...styles.tab,
            ...(activeTab === 'add' ? styles.tabActive : {})
          }}
        >
          <Plus size={18} />
          Add Data
        </button>
        <button
          onClick={() => setActiveTab('view')}
          style={{
            ...styles.tab,
            ...(activeTab === 'view' ? styles.tabActive : {})
          }}
        >
          <List size={18} />
          View Records ({allConsumptionData.length})
        </button>
        <button
          onClick={() => setActiveTab('charts')}
          style={{
            ...styles.tab,
            ...(activeTab === 'charts' ? styles.tabActive : {})
          }}
        >
          <BarChart3 size={18} />
          Analytics
        </button>
      </div>

      {/* Add Data Tab */}
      {activeTab === 'add' && (
        <form onSubmit={handleSubmit} style={styles.form}>
        {error && <div style={styles.errorAlert}>{error}</div>}
        {success && <div style={styles.successAlert}>{success}</div>}

        {/* Site Selection */}
        <section style={styles.section}>
          <div style={styles.sectionHeader}>
            <Zap size={24} color="#8DC5AA" />
            <h2 style={styles.sectionTitle}>Select Site</h2>
          </div>
          
          <div style={styles.siteSelectContainer}>
            <label style={styles.label}>Site *</label>
            <select
              value={selectedSiteId}
              onChange={(e) => setSelectedSiteId(e.target.value)}
              style={styles.selectLarge}
              required
            >
              <option value="">-- Select a site --</option>
              {sites.map(site => (
                <option key={site.id} value={site.id}>
                  {site.nom} ({site.typeSite || 'N/A'})
                </option>
              ))}
            </select>
          </div>
        </section>

        {/* Consumption Records */}
        <section style={styles.section}>
          <div style={styles.sectionHeader}>
            <Zap size={24} color="#8DC5AA" />
            <h2 style={styles.sectionTitle}>Consumption Records</h2>
            <button type="button" onClick={addRecord} style={styles.addButton}>
              <Plus size={16} />
              Add Row
            </button>
          </div>

          {consumptionRecords.length === 0 ? (
            <p style={styles.emptyNote}>No records added. Click "Add Row" to start.</p>
          ) : (
            <div style={styles.tableContainer}>
              <table style={styles.table}>
                <thead>
                  <tr style={styles.tableHeaderRow}>
                    <th style={styles.tableHeader}>#</th>
                    <th style={styles.tableHeader}>Energy Type *</th>
                    <th style={styles.tableHeader}>Consumption *</th>
                    <th style={styles.tableHeader}>Unit</th>
                    <th style={styles.tableHeader}>Data Type</th>
                    <th style={styles.tableHeader}>Period Start</th>
                    <th style={styles.tableHeader}>Period End</th>
                    <th style={styles.tableHeader}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {consumptionRecords.map((record, index) => (
                    <tr key={index} className="consumption-table-row" style={styles.tableRow}>
                      <td style={styles.tableCell}>{index + 1}</td>
                      <td style={styles.tableCell}>
                        <select
                          value={record.typeEnergie}
                          onChange={(e) => updateRecord(index, 'typeEnergie', e.target.value)}
                          className="consumption-table-input"
                          style={styles.tableInput}
                          required
                        >
                          {energyTypes.map(type => (
                            <option key={type} value={type}>{type}</option>
                          ))}
                        </select>
                      </td>
                      <td style={styles.tableCell}>
                        <input
                          type="number"
                          value={record.consommationAnnuelle}
                          onChange={(e) => updateRecord(index, 'consommationAnnuelle', e.target.value)}
                          className="consumption-table-input"
                          style={styles.tableInput}
                          min="0.01"
                          step="0.01"
                          required
                        />
                      </td>
                      <td style={styles.tableCell}>
                        <input
                          type="text"
                          value={record.unite}
                          onChange={(e) => updateRecord(index, 'unite', e.target.value)}
                          className="consumption-table-input"
                          style={styles.tableInput}
                          maxLength="20"
                          placeholder="kWh"
                        />
                      </td>
                      <td style={styles.tableCell}>
                        <select
                          value={record.typeDonnee}
                          onChange={(e) => updateRecord(index, 'typeDonnee', e.target.value)}
                          className="consumption-table-input"
                          style={styles.tableInput}
                        >
                          {dataTypes.map(type => (
                            <option key={type} value={type}>{type}</option>
                          ))}
                        </select>
                      </td>
                      <td style={styles.tableCell}>
                        <input
                          type="datetime-local"
                          value={record.periodeDebut.substring(0, 16)}
                          onChange={(e) => updateRecord(index, 'periodeDebut', e.target.value + ':00.000Z')}
                          className="consumption-table-input"
                          style={styles.tableInput}
                        />
                      </td>
                      <td style={styles.tableCell}>
                        <input
                          type="datetime-local"
                          value={record.periodeFin.substring(0, 16)}
                          onChange={(e) => updateRecord(index, 'periodeFin', e.target.value + ':00.000Z')}
                          className="consumption-table-input"
                          style={styles.tableInput}
                        />
                      </td>
                      <td style={styles.tableCell}>
                        {consumptionRecords.length > 1 && (
                          <button
                            type="button"
                            onClick={() => removeRecord(index)}
                            className="delete-icon-button"
                            style={styles.deleteIconButton}
                            title="Delete row"
                          >
                            <Trash2 size={18} />
                          </button>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </section>

        <div style={styles.actions}>
          <button 
            type="button" 
            onClick={() => navigate('/dashboard')} 
            style={styles.cancelButton}
            disabled={saving}
          >
            Cancel
          </button>
          <button 
            type="submit" 
            style={styles.submitButton} 
            disabled={saving || !selectedSiteId}
          >
            <Save size={18} />
            {saving ? 'Saving...' : `Save ${consumptionRecords.length} Record(s)`}
          </button>
        </div>
      </form>
      )}

      {/* View Records Tab */}
      {activeTab === 'view' && (
        <div style={styles.viewContainer}>
          <section style={styles.section}>
            <div style={styles.sectionHeader}>
              <List size={24} color="#8DC5AA" />
              <h2 style={styles.sectionTitle}>All Consumption Records</h2>
            </div>

            {allConsumptionData.length === 0 ? (
              <p style={styles.emptyNote}>No consumption records found. Add some data first.</p>
            ) : (
              <div style={styles.tableContainer}>
                <table style={styles.table}>
                  <thead>
                    <tr style={styles.tableHeaderRow}>
                      <th style={styles.tableHeader}>Site</th>
                      <th style={styles.tableHeader}>Type</th>
                      <th style={styles.tableHeader}>Energy Type</th>
                      <th style={styles.tableHeader}>Consumption</th>
                      <th style={styles.tableHeader}>Unit</th>
                      <th style={styles.tableHeader}>Data Type</th>
                      <th style={styles.tableHeader}>Period Start</th>
                      <th style={styles.tableHeader}>Period End</th>
                    </tr>
                  </thead>
                  <tbody>
                    {allConsumptionData.map((record, index) => (
                      <tr key={index} className="consumption-table-row" style={styles.tableRow}>
                        <td style={styles.tableCell}>{record.siteName}</td>
                        <td style={styles.tableCell}>
                          <span style={styles.badge}>{record.siteType || 'N/A'}</span>
                        </td>
                        <td style={styles.tableCell}>
                          <strong>{record.typeEnergie}</strong>
                        </td>
                        <td style={styles.tableCell}>
                          {record.consommationAnnuelle?.toLocaleString()}
                        </td>
                        <td style={styles.tableCell}>{record.unite}</td>
                        <td style={styles.tableCell}>
                          <span style={styles.dataTypeBadge}>{record.typeDonnee}</span>
                        </td>
                        <td style={styles.tableCell}>
                          {record.periodeDebut ? new Date(record.periodeDebut).toLocaleDateString() : 'N/A'}
                        </td>
                        <td style={styles.tableCell}>
                          {record.periodeFin ? new Date(record.periodeFin).toLocaleDateString() : 'N/A'}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </section>
        </div>
      )}

      {/* Analytics Tab */}
      {activeTab === 'charts' && (
        <div style={styles.chartsContainer}>
          {allConsumptionData.length === 0 ? (
            <section style={styles.section}>
              <p style={styles.emptyNote}>No consumption data available for analytics.</p>
            </section>
          ) : (
            <>
              <div style={styles.chartsGrid}>
                <div style={styles.chartCard}>
                  <HighchartsReactComponent
                    highcharts={Highcharts}
                    options={getConsumptionByTypeChart()}
                  />
                </div>
                <div style={styles.chartCard}>
                  <HighchartsReactComponent
                    highcharts={Highcharts}
                    options={getConsumptionBySiteChart()}
                  />
                </div>
              </div>
              <div style={styles.chartCard}>
                <HighchartsReactComponent
                  highcharts={Highcharts}
                  options={getDataTypeDistributionChart()}
                />
              </div>
            </>
          )}
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
  tabs: {
    display: 'flex',
    gap: '0.5rem',
    marginBottom: '2rem',
    borderBottom: '2px solid #e5e5e5',
  },
  tab: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.5rem',
    padding: '1rem 1.5rem',
    backgroundColor: 'transparent',
    color: '#666',
    border: 'none',
    borderBottom: '3px solid transparent',
    cursor: 'pointer',
    fontSize: '1rem',
    fontWeight: '500',
    transition: 'all 0.2s',
  },
  tabActive: {
    color: '#8DC5AA',
    borderBottomColor: '#8DC5AA',
    fontWeight: '600',
  },
  form: {
    display: 'flex',
    flexDirection: 'column',
    gap: '2rem',
  },
  errorAlert: {
    backgroundColor: '#ffebee',
    color: '#c62828',
    padding: '1rem',
    borderRadius: '8px',
    fontSize: '0.9rem',
    border: '1px solid #ffcdd2',
  },
  successAlert: {
    backgroundColor: '#e8f5e9',
    color: '#2e7d32',
    padding: '1rem',
    borderRadius: '8px',
    fontSize: '0.9rem',
    border: '1px solid #c8e6c9',
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
  siteSelectContainer: {
    maxWidth: '600px',
  },
  label: {
    fontSize: '0.95rem',
    color: '#555',
    fontWeight: '500',
    marginBottom: '0.5rem',
    display: 'block',
  },
  selectLarge: {
    width: '100%',
    padding: '0.85rem',
    borderRadius: '10px',
    border: '1px solid #ddd',
    fontSize: '1rem',
    outline: 'none',
    transition: 'all 0.2s',
    backgroundColor: '#f9f9f9',
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
  emptyNote: {
    color: '#999',
    fontSize: '0.9rem',
    fontStyle: 'italic',
    textAlign: 'center',
    marginTop: '1rem',
  },
  tableContainer: {
    overflowX: 'auto',
    marginTop: '1rem',
  },
  table: {
    width: '100%',
    borderCollapse: 'collapse',
    fontSize: '0.9rem',
  },
  tableHeaderRow: {
    backgroundColor: '#f8f9fa',
  },
  tableHeader: {
    padding: '1rem 0.75rem',
    textAlign: 'left',
    fontWeight: '600',
    color: '#555',
    borderBottom: '2px solid #ddd',
    whiteSpace: 'nowrap',
  },
  tableRow: {
    borderBottom: '1px solid #e5e5e5',
    transition: 'background-color 0.2s',
  },
  tableCell: {
    padding: '0.75rem',
    verticalAlign: 'middle',
  },
  tableInput: {
    width: '100%',
    padding: '0.5rem',
    border: '1px solid #ddd',
    borderRadius: '6px',
    fontSize: '0.9rem',
    outline: 'none',
    transition: 'border-color 0.2s',
    backgroundColor: 'white',
  },
  deleteIconButton: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    padding: '0.5rem',
    backgroundColor: 'transparent',
    color: '#F44336',
    border: 'none',
    borderRadius: '6px',
    cursor: 'pointer',
    transition: 'background-color 0.2s',
  },
  viewContainer: {
    marginTop: '1rem',
  },
  chartsContainer: {
    marginTop: '1rem',
  },
  chartsGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fit, minmax(450px, 1fr))',
    gap: '2rem',
    marginBottom: '2rem',
  },
  chartCard: {
    backgroundColor: 'white',
    padding: '1.5rem',
    borderRadius: '16px',
    boxShadow: '0 4px 6px rgba(0,0,0,0.05)',
    border: '1px solid #eaeaea',
  },
  badge: {
    padding: '0.25rem 0.75rem',
    backgroundColor: '#E3F2FD',
    color: '#1976D2',
    borderRadius: '12px',
    fontSize: '0.8rem',
    fontWeight: '500',
  },
  dataTypeBadge: {
    padding: '0.25rem 0.75rem',
    backgroundColor: '#F3E5F5',
    color: '#7B1FA2',
    borderRadius: '12px',
    fontSize: '0.8rem',
    fontWeight: '500',
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
