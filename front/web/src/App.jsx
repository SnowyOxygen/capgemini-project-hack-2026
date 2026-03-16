import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from './assets/vite.svg'
import heroImg from './assets/hero.png'
import './App.css'

function App() {
  return (
    <>
      <section id="center" style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh', flexDirection: 'column' }}>
        <h1 style={{ fontSize: '3rem', fontWeight: 'bold' }}>Hello World!</h1>
        <p style={{ fontSize: '1.2rem', color: '#666' }}>This is the web app.</p>
      </section>
    </>
  )
}

export default App
