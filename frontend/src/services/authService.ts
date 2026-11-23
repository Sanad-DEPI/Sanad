import type { ApiResponse } from '../types/api'
import type { AuthenticatedUser, LoginResponse } from '../types/auth'
import { api } from './api'
import { clearAccessToken, setAccessToken } from './authStorage'

export async function login(email: string, password: string): Promise<AuthenticatedUser> {
  const { data } = await api.post<ApiResponse<LoginResponse>>('/auth/login', { email, password })

  if (!data.succeeded || !data.data) {
    throw new Error(data.message ?? 'Login failed. Please try again.')
  }

  setAccessToken(data.data.token)
  return {
    email: data.data.email,
    roles: [...data.data.roles],
    expiresAtUtc: data.data.expiresAtUtc,
  }
}

export function logout(): void {
  clearAccessToken()
}
