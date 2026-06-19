import { Navigate, Outlet } from "react-router-dom";
import { useAuthContext } from "@/context/AuthContext";
import { LoadingSpinner } from "../feedback/Spinner";

function ProtectedRoute() {
  const { isAuthenticated, loading } = useAuthContext();

  if (loading) return <LoadingSpinner />;

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
}

export default ProtectedRoute;
