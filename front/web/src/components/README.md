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

---

### EnergyConsumptionChart

A pie chart component that displays energy consumption breakdown by energy type.

**Usage:**
```jsx
import EnergyConsumptionChart, { calculateTotalEnergy } from '../components/EnergyConsumptionChart';

// In your component
<EnergyConsumptionChart 
  energies={site.energies}
  title="Energy Consumption by Type"  // optional
  height={300}                         // optional
/>

// To calculate total energy
const totalEnergy = calculateTotalEnergy(site.energies);
```

**Props:**
- `energies` (Array, required): Array of energy objects with `typeEnergie`, `consommationAnnuelle`, and `unite` properties
- `title` (String, optional): Chart title (default: "Energy Consumption by Type")
- `height` (Number, optional): Chart height in pixels (default: 300)

**Exported utilities:**
- `calculateTotalEnergy(energies)`: Returns total energy consumption
- `getEnergyByType(energies)`: Returns energy grouped by type

---

### ParkingDistributionChart

A stacked bar chart component that displays parking space distribution by type.

**Usage:**
```jsx
import ParkingDistributionChart, { calculateTotalParking } from '../components/ParkingDistributionChart';

// In your component
<ParkingDistributionChart 
  parking={site.parking}
  title="Parking Distribution"  // optional
  height={300}                   // optional
/>

// To calculate total parking
const totalParking = calculateTotalParking(site.parking);
```

**Props:**
- `parking` (Object, required): Parking object with `placesAeriennes`, `placesSousDalle`, and `placesSousSol` properties
- `title` (String, optional): Chart title (default: "Parking Distribution")
- `height` (Number, optional): Chart height in pixels (default: 300)

**Exported utilities:**
- `calculateTotalParking(parking)`: Returns total parking spaces

---

### BuildingEfficiencyGauge

A gauge chart component that displays building efficiency metrics.

**Usage:**
```jsx
import BuildingEfficiencyGauge, { calculateCO2PerM2, calculateEnergyPerPerson } from '../components/BuildingEfficiencyGauge';

// In your component
<BuildingEfficiencyGauge 
  value={co2PerM2}
  metric="CO2/m²"
  unit="kg/m²"
  title="CO2 Efficiency"  // optional
  height={250}            // optional
  max={500}               // optional
/>

// Calculate efficiency metrics
const co2PerM2 = calculateCO2PerM2(totalCO2, site.superficieM2);
const energyPerPerson = calculateEnergyPerPerson(totalEnergy, site.nombrePersonnes);
```

**Props:**
- `value` (Number, required): The efficiency value to display
- `metric` (String, required): Metric name (e.g., 'CO2/m²', 'Energy/Person')
- `unit` (String, required): Unit of measurement
- `title` (String, optional): Chart title (default: "Building Efficiency")
- `height` (Number, optional): Chart height in pixels (default: 250)
- `max` (Number, optional): Maximum value for gauge (auto-calculated if not provided)

**Exported utilities:**
- `calculateCO2PerM2(totalCO2, superficie)`: Returns CO2 per square meter in kg/m²
- `calculateEnergyPerPerson(totalEnergy, nombrePersonnes)`: Returns energy per person

**Note:** Requires `highcharts-more` and `solid-gauge` modules.

---

### MaterialQuantityChart

A horizontal bar chart component that displays material quantities.

**Usage:**
```jsx
import MaterialQuantityChart, { calculateTotalMaterialWeight, getHeaviestMaterial } from '../components/MaterialQuantityChart';

// In your component
<MaterialQuantityChart 
  materiaux={site.materiaux}
  title="Material Quantities"  // optional
  height={300}                  // optional
/>

// Calculate material statistics
const totalWeight = calculateTotalMaterialWeight(site.materiaux);
const heaviest = getHeaviestMaterial(site.materiaux);
```

**Props:**
- `materiaux` (Array, required): Array of material objects with `materiauNom`, `quantite`, and `unite` properties
- `title` (String, optional): Chart title (default: "Material Quantities")
- `height` (Number, optional): Chart height in pixels (default: 300)

**Exported utilities:**
- `calculateTotalMaterialWeight(materiaux)`: Returns total material weight
- `getHeaviestMaterial(materiaux)`: Returns material with highest quantity

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
