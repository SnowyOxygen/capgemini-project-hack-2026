import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';

// Import and initialize modules - these auto-register with Highcharts
import 'highcharts/highcharts-more';
import 'highcharts/modules/solid-gauge';

// Ensure we're using the default export correctly
const HighchartsReactComponent = HighchartsReact.default || HighchartsReact;

/**
 * Building Efficiency Gauge Component
 * 
 * Displays a gauge showing building efficiency metric (CO2 per m²)
 * 
 * @param {Object} props
 * @param {number} props.value - The efficiency value to display
 * @param {string} props.metric - Metric name (e.g., 'CO2/m²', 'Energy/Person')
 * @param {string} props.unit - Unit of measurement
 * @param {string} props.title - Chart title (optional, defaults to 'Building Efficiency')
 * @param {number} props.height - Chart height in pixels (optional, defaults to 250)
 * @param {number} props.max - Maximum value for gauge (optional, auto-calculated)
 */
export default function BuildingEfficiencyGauge({ 
  value, 
  metric = 'CO2/m²',
  unit = 'kg/m²',
  title = 'Building Efficiency', 
  height = 250,
  max = null
}) {
  
  /**
   * Determine color based on efficiency value
   * Lower is better for CO2 emissions
   */
  const getColor = (val, maxVal) => {
    const percentage = (val / maxVal) * 100;
    if (percentage < 33) return '#4CAF50'; // Green - Good
    if (percentage < 66) return '#FF9800'; // Orange - Medium
    return '#F44336'; // Red - High
  };

  /**
   * Generate Highcharts gauge configuration
   * @returns {Object} Highcharts options object
   */
  const getGaugeOptions = () => {
    const maxValue = max || Math.max(value * 1.5, 100);
    const color = getColor(value, maxValue);
    
    return {
      chart: {
        type: 'solidgauge',
        height: height
      },
      title: {
        text: title,
        style: {
          fontSize: '16px',
          fontWeight: 'bold'
        }
      },
      pane: {
        center: ['50%', '85%'],
        size: '140%',
        startAngle: -90,
        endAngle: 90,
        background: {
          backgroundColor: '#EEE',
          innerRadius: '60%',
          outerRadius: '100%',
          shape: 'arc'
        }
      },
      tooltip: {
        enabled: false
      },
      yAxis: {
        min: 0,
        max: maxValue,
        stops: [
          [0.33, '#4CAF50'],
          [0.66, '#FF9800'],
          [1.0, '#F44336']
        ],
        lineWidth: 0,
        tickWidth: 0,
        minorTickInterval: null,
        tickAmount: 2,
        title: {
          text: metric,
          y: -70,
          style: {
            fontSize: '12px',
            color: '#666'
          }
        },
        labels: {
          y: 16,
          style: {
            fontSize: '10px'
          }
        }
      },
      plotOptions: {
        solidgauge: {
          dataLabels: {
            enabled: true,
            borderWidth: 0,
            useHTML: true,
            y: -50
          }
        }
      },
      series: [{
        name: metric,
        data: [value],
        dataLabels: {
          format: '<div style="text-align:center;">' +
                  '<span style="font-size:24px; font-weight: bold;">{y:.1f}</span><br/>' +
                  '<span style="font-size:11px; color: #888;">' + unit + '</span>' +
                  '</div>'
        }
      }],
      credits: {
        enabled: false
      }
    };
  };

  if (value === null || value === undefined || isNaN(value)) {
    return (
      <div style={styles.noDataMessage}>
        Insufficient data for efficiency calculation
      </div>
    );
  }

  return (
    <div style={styles.chartContainer}>
      <HighchartsReactComponent
        highcharts={Highcharts}
        options={getGaugeOptions()}
      />
    </div>
  );
}

/**
 * Calculate CO2 per square meter
 * @param {number} totalCO2 - Total CO2 in tons
 * @param {number} superficie - Surface area in m²
 * @returns {number} CO2 per m² in kg/m²
 */
export function calculateCO2PerM2(totalCO2, superficie) {
  if (!superficie || superficie <= 0) return null;
  return (totalCO2 * 1000) / superficie; // Convert tons to kg
}

/**
 * Calculate energy per person
 * @param {number} totalEnergy - Total energy consumption
 * @param {number} nombrePersonnes - Number of people
 * @returns {number} Energy per person
 */
export function calculateEnergyPerPerson(totalEnergy, nombrePersonnes) {
  if (!nombrePersonnes || nombrePersonnes <= 0) return null;
  return totalEnergy / nombrePersonnes;
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
