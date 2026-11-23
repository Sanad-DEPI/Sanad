import type { ApiResponse } from '../types/api'
import type { CurrentUser } from '../types/auth'
import { api } from './api'

export async function getCurrentUser(): Promise<CurrentUser> {
  const { data } = await api.get<ApiResponse<CurrentUser>>('/me')
  if (!data.succeeded || !data.data) {
    throw new Error(data.message ?? 'Could not load the current user.')
  }
  return data.data
}

export async function getManagerAccess(): Promise<string> {
  const { data } = await api.get<ApiResponse<string>>('/test/manager')
  if (!data.succeeded || !data.data) {
    throw new Error(data.message ?? 'Manager access check failed.')
  }
  return data.data
}
