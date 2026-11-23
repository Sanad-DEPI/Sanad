import { Navigate, Route, Routes } from 'react-router-dom'
import { AccessDeniedPage } from '../features/access/AccessDeniedPage'
import { LoginPage } from '../features/auth/LoginPage'
import { ProtectedRoute } from '../features/auth/ProtectedRoute'
import { RoleRoute } from '../features/auth/RoleRoute'
import { DashboardPage } from '../features/dashboard/DashboardPage'
import { ManagerPage } from '../features/manager/ManagerPage'
import { AppLayout } from '../layouts/AppLayout'
import { AppRoles } from '../types/roles'

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />

      <Route element={<ProtectedRoute />}>
        <Route element={<AppLayout />}>
          <Route index element={<DashboardPage />} />
          <Route
            path="manager"
            element={
              <RoleRoute roles={[AppRoles.Manager]}>
                <ManagerPage />
              </RoleRoute>
            }
          />
          <Route path="access-denied" element={<AccessDeniedPage />} />
        </Route>
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}
