import 'bootstrap/dist/css/bootstrap.min.css';
import './themes/Spacelab/bootstrap.css';
import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter, Routes, Route } from 'react-router';
import AuthProvider from './components/auth/context/AuthProvider.tsx';
import MainLayout from './layout/MainLayout.tsx';
import NotFoundLayout from './layout/NotFoundLayout.tsx';
import LoginForm from './components/auth/LoginForm.tsx';

const queryClient = new QueryClient();

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <BrowserRouter>
          <Routes>
            <Route path="/" element={<MainLayout />} />
            <Route path="/login" element={<LoginForm />} />
            <Route path="*" element={<NotFoundLayout />} />
          </Routes>
        </BrowserRouter>
      </AuthProvider>
    </QueryClientProvider>
  </StrictMode>,
);
