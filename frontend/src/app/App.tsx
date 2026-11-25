import { Navigate, Route, Routes } from 'react-router-dom'
import { AccessDeniedPage } from '../features/access/AccessDeniedPage'
import { DesignPreviewPage } from '../features/design/DesignPreviewPage'
import { ManagerPage } from '../features/manager/ManagerPage'
import { AppLayout } from '../layouts/AppLayout'

export default function App() {
  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route index element={<DesignPreviewPage />} />
        <Route path="manager" element={<ManagerPage />} />
        <Route path="access-denied" element={<AccessDeniedPage />} />
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}
