import { Routes, Route } from "react-router-dom";
import Layout from "@/components/layout/Layout";
import Dashboard from "@/pages/Dashboard";
import History from "@/pages/History";
import Programs from "@/pages/Programs";
import Progress from "@/pages/Progress";
import Login from "@/pages/Login";
import Exercises from "@/pages/Exercises";
import Settings from "@/pages/Settings";

function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route index element={<Dashboard />} />
        <Route path="programs" element={<Programs />} />
        <Route path="exercises" element={<Exercises />} />
        <Route path="history" element={<History />} />
        <Route path="progress" element={<Progress />} />
        <Route path="settings" element={<Settings />} />
      </Route>
      <Route path="login" element={<Login />} />
    </Routes>
  );
}

export default App;
