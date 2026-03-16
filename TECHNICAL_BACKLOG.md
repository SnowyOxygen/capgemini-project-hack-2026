# Technical Backlog - Carbon Footprint Tracking System

**Stack:** .NET 10 API + React Frontend  
**Current Status:** Data models and repositories implemented, controllers and frontend needed

---

## Version 1 - MVP

### 1. Authentication & Account (P0)

**Backend:**
- ✅ JWT authentication & password hashing
- ✅ Sign in/Sign up endpoints
- ✅ Password reset endpoints (forgot-password, reset-password)
- ✅ Email service integration
- ✅ Token blacklist for logout

**Frontend:**
- ⬜ Login/Logout pages
- ⬜ Password reset flow
- ⬜ Auth context & protected routes
- ⬜ Token storage & refresh logic

---

### 2. Site Management (P0)

**Backend:**
- ✅ Site, Parking, Energie, Materiau, SiteMateriau models
- ✅ All repositories implemented
- ⬜ SiteController: CRUD endpoints (/api/sites)
- ⬜ ParkingController: CRUD endpoints (/api/sites/{id}/parkings)
- ⬜ EnergieController: CRUD endpoints (/api/sites/{id}/energies)
- ⬜ MateriauController: GET all materials (/api/materiaux)
- ⬜ SiteMateriauController: CRUD (/api/sites/{id}/materiaux)
- ⬜ DTOs & mappers for all entities
- ⬜ Authorization filters (user can only access their sites)
- ⬜ Seed reference data (materials, energy factors)

**Frontend:**
- ⬜ Site list page
- ⬜ Multi-step site creation form:
  - Basic info: Nom, TypeSite, AnneeConstruction, SuperficieM2, NombrePersonnes
  - Parking: NombrePlacesTotal
  - Energy: Consommation électrique/gaz (Réelle/Estimée)
  - Materials: Add materials with quantities (kg/tonnes)
- ⬜ Site detail view
- ⬜ Form validation & error handling
- ⬜ API client services

---

### 3. Emission Calculation Engine (P0)

**Backend:**
- ✅ EmissionSnapshot & EmissionMensuelle models
- ✅ Repositories implemented
- ⬜ IEmissionCalculationService interface & implementation:
  - Calculate construction emissions: Σ(Materiau.Quantite × FacteurEmission)
  - Calculate energy emissions: Energy × FacteurEnergie
  - Calculate total, annual, per m², per person
- ⬜ EmissionController:
  - POST /api/sites/{id}/calculate-emissions
  - GET /api/sites/{id}/emissions/latest
- ⬜ DTOs & mappers

**Frontend:**
- ⬜ "Calculate Emissions" button
- ⬜ Display results: Total & annual carbon footprint
- ⬜ Loading states & error handling

---

### 4. Emission History (P0)

**Backend:**
- ⬜ EmissionHistoryController endpoints:
  - GET /api/sites/{id}/emissions/history/rolling-12 (12 months)
  - GET /api/sites/{id}/emissions/history/current-year
  - GET /api/sites/{id}/emissions/history/previous-year
  - GET /api/sites/{id}/emissions/history/rolling-24 (24 months table)
  - GET /api/sites/{id}/emissions/history?start=X&end=Y (date range)
- ⬜ EmissionHistoryService for aggregation logic
- ⬜ DTOs for monthly emission records

**Frontend:**
- ⬜ Emission History page
- ⬜ Line chart: 12-month rolling emissions (default)
- ⬜ Toggle views: current year / previous year
- ⬜ Data table: 24-month emissions
- ⬜ Date range filter
- ⬜ Chart library setup (Chart.js or Recharts)

---

## Version 2 - Dashboard & Enhancements

### 5. Dashboard (P1)

**Backend:**
- ⬜ DashboardController:
  - GET /api/sites/{id}/dashboard (aggregate all data)
  - GET /api/sites/{id}/emissions/breakdown (% by source)
- ⬜ Dashboard service for data aggregation
- ⬜ DTOs: EmissionBreakdownDto, DashboardDto

**Frontend:**
- ⬜ Dashboard page with KPI cards:
  - Total & annual carbon footprint
  - CO₂ per m² / per employee
  - Site info (Nom, AnneeConstruction)
- ⬜ Pie chart: Emission breakdown (Construction, Electric, Gas %)
- ⬜ Line chart: 12-month rolling emissions overview
- ⬜ Responsive grid layout

---

### 6. Enhanced Features (P1)

**Backend:**
- ⬜ Add NombreEtages to site form
- ⬜ Parking breakdown: PlacesAeriennes, PlacesSousDalle, PlacesSousSol
- ⬜ Display material emission factors in responses

**Frontend:**
- ⬜ Add "Nombre d'étages" field to site form
- ⬜ Parking breakdown fields (Aériennes, Sous-dalle, Sous-sol)
- ⬜ Display emission factors for materials
- ⬜ Validation: sum of parking types = total

---

## Version 3 - Comparison & Analytics

### 7. Site Comparison (P2)

**Backend:**
- ⬜ ComparisonController:
  - GET /api/comparison?siteId1=X&siteId2=Y (compare two sites)
  - GET /api/comparison/history?siteId1=X&siteId2=Y (emission history)
- ⬜ ComparisonService for side-by-side comparison
- ⬜ DTOs: SiteComparisonDto, TwoSiteHistoryComparisonDto

**Frontend:**
- ⬜ Site Comparison page
- ⬜ Dual site selector
- ⬜ Side-by-side property comparison:
  - Basic info, parking, energy, materials, people
- ⬜ KPI comparison: Total & annual emissions
- ⬜ Dual pie charts: Emission breakdown by source
- ⬜ Dual line chart: 12-month rolling comparison
- ⬜ Color coding (green/red) for better/worse

---

### 8. Historical Comparison (P2)

**Backend:**
- ⬜ YearOverYearController:
  - GET /api/comparison/{id}/year-over-year?start=X&end=Y
  - Calculate % change: (Current - Previous) / Previous × 100

**Frontend:**
- ⬜ Y-o-Y comparison overlay on history chart
- ⬜ % increase/decrease indicator (with arrows)
- ⬜ Date range selector for period comparison

---

## Cross-Cutting Concerns

### Infrastructure (P0-P1)

**Backend:**
- ✅ EF Core with DataContext
- ⬜ CORS configuration for React
- ⬜ Global exception handling middleware
- ⬜ Swagger/OpenAPI documentation
- ⬜ Dependency injection setup in Program.cs
- ⬜ Model validation filter
- ⬜ Health check endpoint (/api/health)
- ⬜ Request/response logging (Serilog)

**Frontend:**
- ⬜ React + TypeScript project setup (Vite or CRA)
- ⬜ React Router for navigation
- ⬜ State management (Context API or Redux)
- ⬜ Axios base configuration with interceptors
- ⬜ Form library (React Hook Form or Formik)
- ⬜ UI component library (Material-UI or Ant Design)
- ⬜ Toast notifications
- ⬜ Error boundary components
- ⬜ Environment variables config

---

### Database (P0-P1)

- ✅ All migrations created
- ⬜ Add indexes on foreign keys (SiteId, UserId, MateriauId)
- ⬜ User-Site relationship (add UserId to Site model)
- ⬜ Cascading delete rules
- ⬜ Soft delete support (IsDeleted flag)
- ⬜ Audit fields (CreatedBy, UpdatedBy, UpdatedAt)
- ⬜ Database seeding for dev/test environments
- ⬜ Background job for monthly emission tracking

---

### Security (P0)

- ⬜ Input sanitization & XSS protection
- ⬜ HTTPS enforcement
- ⬜ Rate limiting
- ⬜ Proper password policies (min length, complexity)
- ⬜ Secure token storage (HttpOnly cookies or secure storage)
- ⬜ API request size limits
- ⬜ Authorization checks on all endpoints

---

### Performance (P1)

- ⬜ Database connection pooling
- ⬜ Caching for reference data (materials, energy factors)
- ⬜ Eager loading for navigation properties (Include())
- ⬜ Pagination for list endpoints
- ⬜ Request debouncing on frontend search
- ⬜ Lazy loading for React routes (code splitting)
- ⬜ Chart rendering optimization

---

### Testing (P1)

**Backend:**
- ⬜ Unit tests for EmissionCalculationService
- ⬜ Unit tests for repositories
- ⬜ Integration tests for API endpoints
- ⬜ Test database setup (in-memory or container)

**Frontend:**
- ⬜ Unit tests for API client services
- ⬜ Component tests for forms & dashboard
- ⬜ E2E tests for critical paths (Cypress/Playwright)
- ⬜ Test coverage >80%

---

### Deployment (P1)

- ⬜ Dockerfile for API
- ⬜ Dockerfile for React app (Nginx)
- ⬜ Docker Compose configuration
- ⬜ Environment configuration (Dev, Staging, Prod)
- ⬜ CI/CD pipeline (GitHub Actions or Azure DevOps)
- ⬜ README with setup instructions
- ⬜ API documentation
- ⬜ Architecture documentation
- ⬜ Monitoring & alerting setup

---

## Priority Legend

- **P0**: Critical - Must have for MVP
- **P1**: High - Important for full functionality
- **P2**: Medium - Can be deferred to later versions

---

## Implementation Phases

### Phase 1: MVP (Version 1)
Focus on core CRUD, emission calculation, and basic history tracking.  
**Est. Duration:** 6-8 weeks

### Phase 2: Dashboard (Version 2)
Add comprehensive dashboard with visualizations and enhanced features.  
**Est. Duration:** 3-4 weeks

### Phase 3: Comparison (Version 3)
Implement site comparison and advanced analytics.  
**Est. Duration:** 3-4 weeks

---

## Key Dependencies

1. **User-Site Relationship**: Add `UserId` to Site model to link sites to users
2. **Reference Data Seeding**: Materials and energy emission factors must be seeded before calculations work
3. **Frontend Chart Library**: Choose and setup early (impacts all visualization work)
4. **Email Service**: Required for password reset functionality

---

## Current Status Summary

✅ **Done:**
- All data models created
- Repository pattern implemented
- Basic auth infrastructure (JWT, password hashing)
- Database migrations

🚧 **Next Steps:**
1. Add UserId to Site model & migration
2. Create all controllers with DTOs
3. Implement EmissionCalculationService
4. Setup React project
5. Seed reference data

❌ **Not Started:**
- All controllers
- Business logic services
- Frontend application
- Testing
- Deployment
