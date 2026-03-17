# Chart Components

This directory contains reusable chart components for displaying site analytics.

## Available Charts

### CO2EmissionsChart

A pie chart component that displays CO2 emissions breakdown by construction materials.

**Usage:**
```jsx
import CO2EmissionsChart, { calculateTotalCO2 } from '../components/CO2EmissionsChart';

// In your component
<CO2EmissionsChart 
  materiaux={site.materiaux}
  title="CO2 Emissions by Material"  // optional
  height={300}                        // optional
/>

// To calculate total CO2
const totalCO2 = calculateTotalCO2(site.materiaux);
```

**Props:**
- `materiaux` (Array, required): Array of material objects with `materiauNom`, `quantite`, and `unite` properties
- `title` (String, optional): Chart title (default: "CO2 Emissions by Material")
- `height` (Number, optional): Chart height in pixels (default: 300)

**Exported utilities:**
- `calculateTotalCO2(materiaux)`: Returns total CO2 emissions in tons
- `CO2_FACTORS`: Object containing CO2 emission factors for different materials

## Adding New Charts

To add a new chart component:

1. **Create a new file** in this directory (e.g., `EnergyConsumptionChart.jsx`)

2. **Follow this structure:**
```jsx
import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';

const HighchartsReactComponent = HighchartsReact.default || HighchartsReact;

export default function YourChartComponent({ data, title, height = 300 }) {
  
  const getChartOptions = () => {
    return {
      chart: {
        type: 'bar', // or 'line', 'column', etc.
        height: height
      },
      title: {
        text: title,
        style: {
          fontSize: '16px',
          fontWeight: 'bold'
        }
      },
      // ... your chart configuration
      credits: {
        enabled: false
      }
    };
  };

  return (
    <div style={{ marginTop: '1rem', padding: '0.5rem' }}>
      <HighchartsReactComponent
        highcharts={Highcharts}
        options={getChartOptions()}
      />
    </div>
  );
}
```

3. **Import and use** in your page component:
```jsx
import YourChartComponent from '../components/YourChartComponent';

// In your JSX
<YourChartComponent data={yourData} title="Your Chart Title" />
```

## Highcharts Documentation

- [Highcharts Demo Gallery](https://www.highcharts.com/demo)
- [API Reference](https://api.highcharts.com/highcharts/)
- [Chart Types](https://www.highcharts.com/docs/chart-and-series-types/chart-types)

## Tips

- Keep chart components **focused and reusable**
- **Export utility functions** that might be useful in parent components
- Add **JSDoc comments** to document props and functions
- Handle **empty data gracefully** with appropriate messaging
- Use **consistent styling** across all chart components
