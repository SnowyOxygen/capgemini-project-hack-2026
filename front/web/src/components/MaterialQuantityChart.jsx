import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';

// Ensure we're using the default export correctly
const HighchartsReactComponent = HighchartsReact.default || HighchartsReact;

/**
 * Material Quantity Chart Component
 * 
 * Displays a horizontal bar chart showing material quantities
 * 
 * @param {Object} props
 * @param {Array} props.materiaux - Array of material objects with materiauNom, quantite, and unite
 * @param {string} props.title - Chart title (optional, defaults to 'Material Quantities')
 * @param {number} props.height - Chart height in pixels (optional, defaults to 300)
 */
export default function MaterialQuantityChart({ materiaux, title = 'Material Quantities', height = 300 }) {
  
  /**
   * Prepare material data for the chart
   * @returns {Object} Categories and data arrays
   */
  const getMaterialData = () => {
    if (!materiaux || materiaux.length === 0) {
      return { categories: [], data: [], units: [] };
    }

    const categories = [];
    const data = [];
    const colors = [];
    const units = [];

    // Color palette for materials
    const colorPalette = [
      '#8DC5AA', '#2196F3', '#FF9800', '#4CAF50', 
      '#F44336', '#9C27B0', '#00BCD4', '#FFC107'
    ];

    materiaux.forEach((material, index) => {
      if (material.quantite > 0) {
        categories.push(material.materiauNom || 'Unknown');
        data.push({
          y: material.quantite,
          color: colorPalette[index % colorPalette.length],
          unit: material.unite || 't'
        });
        units.push(material.unite || 't');
      }
    });

    return { categories, data, units };
  };

  /**
   * Generate Highcharts configuration options
   * @returns {Object} Highcharts options object
   */
  const getBarChartOptions = () => {
    const { categories, data } = getMaterialData();
    
    return {
      chart: {
        type: 'bar',
        height: height
      },
      title: {
        text: title,
        style: {
          fontSize: '16px',
          fontWeight: 'bold'
        }
      },
      xAxis: {
        categories: categories,
        title: {
          text: null
        }
      },
      yAxis: {
        min: 0,
        title: {
          text: 'Quantity',
          align: 'high'
        },
        labels: {
          overflow: 'justify'
        }
      },
      tooltip: {
        pointFormat: '<b>{point.y:.1f} {point.unit}</b>'
      },
      plotOptions: {
        bar: {
          dataLabels: {
            enabled: true,
            format: '{point.y:.1f} {point.unit}'
          },
          colorByPoint: true
        }
      },
      legend: {
        enabled: false
      },
      series: [{
        name: 'Quantity',
        data: data
      }],
      credits: {
        enabled: false
      }
    };
  };

  const { categories } = getMaterialData();

  if (categories.length === 0) {
    return (
      <div style={styles.noDataMessage}>
        No material data available
      </div>
    );
  }

  return (
    <div style={styles.chartContainer}>
      <HighchartsReactComponent
        highcharts={Highcharts}
        options={getBarChartOptions()}
      />
    </div>
  );
}

/**
 * Utility function to get the heaviest material
 * @param {Array} materiaux - Array of material objects
 * @returns {Object} Material with highest quantity
 */
export function getHeaviestMaterial(materiaux) {
  if (!materiaux || materiaux.length === 0) return null;
  
  return materiaux.reduce((max, material) => {
    return (material.quantite || 0) > (max.quantite || 0) ? material : max;
  });
}

/**
 * Utility function to calculate total material weight
 * @param {Array} materiaux - Array of material objects
 * @returns {number} Total material quantity
 */
export function calculateTotalMaterialWeight(materiaux) {
  if (!materiaux || materiaux.length === 0) return 0;
  
  return materiaux.reduce((total, material) => {
    return total + (material.quantite || 0);
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
