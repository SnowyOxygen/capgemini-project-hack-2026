import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';

// Ensure we're using the default export correctly
const HighchartsReactComponent = HighchartsReact.default || HighchartsReact;

/**
 * Parking Distribution Chart Component
 * 
 * Displays a stacked bar chart showing parking distribution by type
 * 
 * @param {Object} props
 * @param {Object} props.parking - Parking object with placesAeriennes, placesSousDalle, and placesSousSol
 * @param {string} props.title - Chart title (optional, defaults to 'Parking Distribution')
 * @param {number} props.height - Chart height in pixels (optional, defaults to 300)
 */
export default function ParkingDistributionChart({ parking, title = 'Parking Distribution', height = 300 }) {
  
  /**
   * Prepare parking data for the chart
   * @returns {Object} Categories and series data
   */
  const getParkingData = () => {
    if (!parking) {
      return { categories: [], series: [] };
    }

    const aerial = parking.placesAeriennes || 0;
    const underSlab = parking.placesSousDalle || 0;
    const underground = parking.placesSousSol || 0;
    const total = parking.nombrePlacesTotal || (aerial + underSlab + underground);

    return {
      categories: ['Parking Spaces'],
      series: [
        {
          name: 'Aerial',
          data: [aerial],
          color: '#2196F3'
        },
        {
          name: 'Under-Slab',
          data: [underSlab],
          color: '#FF9800'
        },
        {
          name: 'Underground',
          data: [underground],
          color: '#4CAF50'
        }
      ],
      total: total
    };
  };

  /**
   * Generate Highcharts configuration options
   * @returns {Object} Highcharts options object
   */
  const getBarChartOptions = () => {
    const { categories, series, total } = getParkingData();
    
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
        categories: categories
      },
      yAxis: {
        min: 0,
        title: {
          text: 'Number of Spaces'
        },
        stackLabels: {
          enabled: true,
          style: {
            fontWeight: 'bold',
            color: 'gray'
          }
        }
      },
      legend: {
        align: 'center',
        verticalAlign: 'bottom',
        backgroundColor: 'white',
        borderColor: '#CCC',
        borderWidth: 1,
        shadow: false
      },
      tooltip: {
        headerFormat: '<b>{point.x}</b><br/>',
        pointFormat: '{series.name}: {point.y}<br/>Total: {point.stackTotal}'
      },
      plotOptions: {
        bar: {
          stacking: 'normal',
          dataLabels: {
            enabled: true
          }
        }
      },
      series: series,
      credits: {
        enabled: false
      }
    };
  };

  if (!parking || (!parking.placesAeriennes && !parking.placesSousDalle && !parking.placesSousSol)) {
    return (
      <div style={styles.noDataMessage}>
        No parking data available
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
 * Utility function to calculate total parking spaces
 * @param {Object} parking - Parking object
 * @returns {number} Total parking spaces
 */
export function calculateTotalParking(parking) {
  if (!parking) return 0;
  
  return (parking.placesAeriennes || 0) + 
         (parking.placesSousDalle || 0) + 
         (parking.placesSousSol || 0);
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
