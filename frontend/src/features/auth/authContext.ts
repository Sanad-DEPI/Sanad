import { createContext, useContext } from 'react'
import type { AuthenticatedUser } from '../../types/auth'

export interface AuthContextValue {
  user: AuthenticatedUser | null
  isAuthenticated: boolean
  login: (email: string, password: string) => Promise<void>
  logout: () => void
  hasRole: (role: string) => boolean
}

export const AuthContext = createContext<AuthContextValue | null>(null)

export function useAuth(): AuthContextValue {
  const value = useContext(AuthContext)
  if (!value) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return value
}
