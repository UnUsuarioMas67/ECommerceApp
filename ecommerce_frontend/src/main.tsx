import 'bootstrap/dist/css/bootstrap.min.css';
import './themes/Cerulean/bootstrap.css';
import { StrictMode } from 'react';
import ReactDOM from 'react-dom/client';
import { RouterProvider, createRouter } from '@tanstack/react-router';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import AuthProvider from './components/AuthProvider';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';

// Import the generated route tree
import { routeTree } from './routeTree.gen';
import { useAuth } from './components/AuthProvider/AuthContext';
import { useAxios } from './hooks/use-axios';
import CartProvider from './components/CartProvider';
import { useCart } from './components/CartProvider/CartContext';

const queryClient = new QueryClient();

// Create a new router instance
const router = createRouter({
  routeTree,
  context: {
    queryClient: undefined!,
    authContext: undefined!,
    axiosInstance: undefined!,
    cartContext: undefined!
  },
});

// Register the router instance for type safety
declare module '@tanstack/react-router' {
  interface Register {
    router: typeof router;
  }
}

// Render the app
const rootElement = document.getElementById('root')!;
if (!rootElement.innerHTML) {
  const root = ReactDOM.createRoot(rootElement);
  root.render(
    <StrictMode>
      <QueryClientProvider client={queryClient}>
        <AuthProvider>
          <CartProvider>
            <App />
          </CartProvider>
        </AuthProvider>
        <ReactQueryDevtools />
      </QueryClientProvider>
    </StrictMode>,
  );
}

function App() {
  const authContext = useAuth();
  const axiosInstance = useAxios();
  const cartContext = useCart();
  return <RouterProvider router={router} context={{ queryClient, authContext, axiosInstance, cartContext }} />;
}
