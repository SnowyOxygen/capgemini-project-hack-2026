import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';

// Ensure we're using the default export correctly
const HighchartsReactComponent = HighchartsReact.default || HighchartsReact;

/**
 * Energy Consumption Breakdown Chart Component
 * 
 * Displays a pie chart showing energy consumption breakdown by energy type
 * 
 * @param {Object} props
 * @param {Array} props.energies - Array of energy objects with typeEnergie, consommationAnnuelle, and unite
 * @param {string} props.title - Chart title (optional, defaults to 'Energy Consumption by Type')
 * @param {number} props.height - Chart height in pixels (optional, defaults to 300)
 */
export default function EnergyConsumptionChart({ energies, title = 'Energy Consumption by Type', height = 300 }) {
  
  /**
   * Prepare energy data for the chart
   * @returns {Array} Array of data points with name, y (consumption), and unit
   */
  const getEnergyData = () => {
    if (!energies || energies.length === 0) {
      return [];
    }

    return energies.map(energy => ({
      name: energy.typeEnergie || 'Unknown',
      y: energy.consommationAnnuelle || 0,
      unit: energy.unite || 'kWh',
      dataType: energy.typeDonnee || 'N/A'
    })).filter(item => item.y > 0);
  };

  /**
   * Generate Highcharts configuration options
   * @returns {Object} Highcharts options object
   */
  const getPieChartOptions = () => {
    const data = getEnergyData();
    
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
                     'Consumption: <b>{point.y:,.0f} {point.unit}</b><br/>' +
                     'Data Type: {point.dataType}'
      },
      plotOptions: {
        pie: {
          allowPointSelect: true,
          cursor: 'pointer',
          dataLabels: {
            enabled: true,
            format: '<b>{point.name}</b>: {point.percentage:.1f}%'
          },
          colors: ['#2196F3', '#FF9800', '#4CAF50', '#F44336', '#9C27B0', '#00BCD4']
        }
      },
      series: [{
        name: 'Energy Consumption',
        colorByPoint: true,
        data: data
      }],
      credits: {
        enabled: false
      }
    };
  };

  const energyData = getEnergyData();

  if (energyData.length === 0) {
    return (
      <div style={styles.noDataMessage}>
        No energy consumption data available
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

/**
 * Utility function to calculate total energy consumption
 * @param {Array} energies - Array of energy objects
 * @returns {number} Total energy consumption
 */
export function calculateTotalEnergy(energies) {
  if (!energies || energies.length === 0) return 0;
  
  return energies.reduce((total, energy) => {
    return total + (energy.consommationAnnuelle || 0);
  }, 0);
}

/**
 * Utility function to get energy consumption by type
 * @param {Array} energies - Array of energy objects
 * @returns {Object} Energy consumption grouped by type
 */
export function getEnergyByType(energies) {
  if (!energies || energies.length === 0) return {};
  
  return energies.reduce((acc, energy) => {
    const type = energy.typeEnergie || 'Unknown';
    if (!acc[type]) {
      acc[type] = 0;
    }
    acc[type] += energy.consommationAnnuelle || 0;
    return acc;
  }, {});
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
