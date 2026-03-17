import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from './assets/vite.svg'
import heroImg from './assets/hero.png'
import { Routes, Route } from 'react-router-dom';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import AddSite from './pages/AddSite';
import AddConsumption from './pages/AddConsumption';
import SiteDetail from './pages/SiteDetail';
import EditSite from './pages/EditSite';
import DashboardLayout from './components/DashboardLayout';
import './App.css';

function App() {
  return (
    <Routes>
      <Route path="/" element={<Login />} />
      <Route path="/register" element={<Register />} />
      
      {/* Protected routes with persistent sidebar */}
      <Route element={<DashboardLayout />}>
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/site/:id" element={<SiteDetail />} />
        <Route path="/site/:id/edit" element={<EditSite />} />
        <Route path="/add-site" element={<AddSite />} />
        <Route path="/add-consumption" element={<AddConsumption />} />
      </Route>
    </Routes>
  )
}

export default App
