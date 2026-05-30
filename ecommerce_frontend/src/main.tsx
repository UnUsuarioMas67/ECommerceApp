import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import 'bootstrap/dist/css/bootstrap.min.css';
import './themes/Spacelab/bootstrap.min.css';
import { BrowserRouter, Routes, Route } from 'react-router';
import HomeLayout from './layout/HomeLayout.tsx';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<HomeLayout />} />
      </Routes>
    </BrowserRouter>
  </StrictMode>,
);
