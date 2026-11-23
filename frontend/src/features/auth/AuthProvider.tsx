import { useCallback, useEffect, useMemo, useState, type ReactNode } from 'react'
import { UNAUTHORIZED_EVENT } from '../../services/api'
import { login as loginRequest, logout as logoutRequest } from '../../services/authService'
import type { AuthenticatedUser } from '../../types/auth'
import { AuthContext, type AuthContextValue } from './authContext'

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthenticatedUser | null>(null)

  const login = useCallback(async (email: string, password: string) => {
    const authenticated = await loginRequest(email, password)
    setUser(authenticated)
  }, [])

  const logout = useCallback(() => {
    logoutRequest()
    setUser(null)
  }, [])

  useEffect(() => {
    const handleUnauthorized = () => setUser(null)
    window.addEventListener(UNAUTHORIZED_EVENT, handleUnauthorized)
    return () => window.removeEventListener(UNAUTHORIZED_EVENT, handleUnauthorized)
  }, [])

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      isAuthenticated: user !== null,
      login,
      logout,
      hasRole: (role: string) => user?.roles.includes(role) ?? false,
    }),
    [user, login, logout],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
