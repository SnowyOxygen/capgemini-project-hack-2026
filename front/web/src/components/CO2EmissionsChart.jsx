import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';

// Ensure we're using the default export correctly
const HighchartsReactComponent = HighchartsReact.default || HighchartsReact;

// CO2 emission factors (kg CO2 per ton of material)
const CO2_FACTORS = {
  'Béton': 180,
  'Acier': 1900,
  'Bois': 150,
  'Verre': 700,
  'Aluminium': 10000,
  'Brique': 250,
  'Ciment': 900,
  'Plâtre': 120,
  'default': 200 // Default for unknown materials
};

/**
 * CO2 Emissions Pie Chart Component
 * 
 * Displays a pie chart showing CO2 emissions breakdown by construction materials
 * 
 * @param {Object} props
 * @param {Array} props.materiaux - Array of material objects with materiauNom, quantite, and unite
 * @param {string} props.title - Chart title (optional, defaults to 'CO2 Emissions by Material')
 * @param {number} props.height - Chart height in pixels (optional, defaults to 300)
 */
export default function CO2EmissionsChart({ materiaux, title = 'CO2 Emissions by Material', height = 300 }) {
  
  /**
   * Calculate CO2 emissions for each material
   * @returns {Array} Array of data points with name, y (CO2 emissions), quantity, and unit
   */
  const calculateCO2Data = () => {
    if (!materiaux || materiaux.length === 0) {
      return [];
    }

    return materiaux.map(material => {
      const factor = CO2_FACTORS[material.materiauNom] || CO2_FACTORS.default;
      const co2Emission = (material.quantite || 0) * factor / 1000; // Convert to tons CO2
      
      return {
        name: material.materiauNom,
        y: co2Emission,
        quantity: material.quantite,
        unit: material.unite
      };
    }).filter(item => item.y > 0);
  };

  /**
   * Generate Highcharts configuration options
   * @returns {Object} Highcharts options object
   */
  const getPieChartOptions = () => {
    const data = calculateCO2Data();
    
    return {
      chart: {
        type: 'pie',
        height: height
      },
      title: {
        text: title,
        style: {
          fontSize: '16px',
          fontWeight: 'bold'
        }
      },
      tooltip: {
        pointFormat: '<b>{point.name}</b><br/>' +
                     'CO2: <b>{point.y:.2f} tons</b><br/>' +
                     'Quantity: {point.quantity} {point.unit}'
      },
      plotOptions: {
        pie: {
          allowPointSelect: true,
          cursor: 'pointer',
          dataLabels: {
            enabled: true,
            format: '<b>{point.name}</b>: {point.percentage:.1f}%'
          }
        }
      },
      series: [{
        name: 'CO2 Emissions',
        colorByPoint: true,
        data: data
      }],
      credits: {
        enabled: false
      }
    };
  };

  /**
   * Calculate total CO2 emissions
   * @returns {number} Total CO2 in tons
   */
  const getTotalCO2 = () => {
    const data = calculateCO2Data();
    return data.reduce((sum, item) => sum + item.y, 0);
  };

  const co2Data = calculateCO2Data();

  if (co2Data.length === 0) {
    return (
      <div style={styles.noDataMessage}>
        No material data available for CO2 calculation
      </div>
    );
  }

  return (
    <div style={styles.chartContainer}>
      <HighchartsReactComponent
        highcharts={Highcharts}
        options={getPieChartOptions()}
      />
    </div>
  );
}

// Export helper functions for use in parent components
export { CO2_FACTORS };

/**
 * Utility function to calculate total CO2 from materials array
 * @param {Array} materiaux - Array of material objects
 * @returns {number} Total CO2 emissions in tons
 */
export function calculateTotalCO2(materiaux) {
  if (!materiaux || materiaux.length === 0) return 0;
  
  return materiaux.reduce((total, material) => {
    const factor = CO2_FACTORS[material.materiauNom] || CO2_FACTORS.default;
    const co2Emission = (material.quantite || 0) * factor / 1000;
    return total + co2Emission;
  }, 0);
}

const styles = {
  chartContainer: {
    marginTop: '1rem',
    padding: '0.5rem',
  },
  noDataMessage: {
    textAlign: 'center',
    padding: '2rem',
    fontSize: '0.9rem',
    color: '#999',
    fontStyle: 'italic',
  }
};
