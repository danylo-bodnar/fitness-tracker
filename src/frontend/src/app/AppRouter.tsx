import { BrowserRouter, Routes, Route } from "react-router-dom";
import Layout from "@/components/layout/Layout";
import ProtectedRoute from "@/components/auth/ProtectedRoute";

import DashboardPage from "@/pages/Dashboard/DashboardPage";
import HistoryPage from "@/pages/History/HistoryPage";
import ProgramsPage from "@/pages/Programs/ProgramsPage";
import ProgressPage from "@/pages/Progress/ProgressPage";
import LoginPage from "@/pages/Login/LoginPage";
import ExercisesPage from "@/pages/Exercises/ExercisesPage";
import SettingsPage from "@/pages/Settings/SettingsPage";
import NotFoundPage from "@/pages/NotFound/NotFoundPage";

function AppRouter() {
  return (
    <BrowserRouter>
      <Routes>
        {/* PUBLIC */}
        <Route path="/login" element={<LoginPage />} />

        {/* PROTECTED */}
        <Route element={<ProtectedRoute />}>
          <Route element={<Layout />}>
            <Route index element={<DashboardPage />} />
            <Route path="programs" element={<ProgramsPage />} />
            <Route path="exercises" element={<ExercisesPage />} />
            <Route path="history" element={<HistoryPage />} />
            <Route path="progress" element={<ProgressPage />} />
            <Route path="settings" element={<SettingsPage />} />
          </Route>
        </Route>

        {/* FALLBACK */}
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default AppRouter;
