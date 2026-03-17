import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from './assets/vite.svg'
import heroImg from './assets/hero.png'
import { Routes, Route } from 'react-router-dom';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import AddSite from './pages/AddSite';
import SiteList from './pages/SiteList';
import './App.css';

function App() {
  return (
    <Routes>
      <Route path="/" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route path="/dashboard" element={<Dashboard />} />
      <Route path="/add-site" element={<AddSite />} />
      <Route path="/sites" element={<SiteList />} />
    </Routes>
  )
}

export default App
