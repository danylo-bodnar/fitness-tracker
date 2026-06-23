import { Navigate, Outlet } from "react-router-dom";
import { useAuthContext } from "@/context/useAuthContext";

function ProtectedRoute() {
  const { isAuthenticated } = useAuthContext();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
}

export default ProtectedRoute;
