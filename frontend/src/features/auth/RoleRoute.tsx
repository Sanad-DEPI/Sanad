import { Navigate } from 'react-router-dom'
import type { ReactNode } from 'react'
import { useAuth } from './authContext'

export function RoleRoute({ roles, children }: { roles: string[]; children: ReactNode }) {
  const { hasRole } = useAuth()
  const allowed = roles.some((role) => hasRole(role))

  if (!allowed) {
    return <Navigate to="/access-denied" replace />
  }

  return <>{children}</>
}
