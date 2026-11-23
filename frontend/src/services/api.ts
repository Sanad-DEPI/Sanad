import axios from 'axios'
import { appConfig } from '../app/config'
import { clearAccessToken, getAccessToken } from './authStorage'

export const UNAUTHORIZED_EVENT = 'sanad:unauthorized'

export const api = axios.create({
  baseURL: appConfig.apiBaseUrl,
  headers: { 'Content-Type': 'application/json' },
})

api.interceptors.request.use((config) => {
  const token = getAccessToken()
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  (response) => response,
  (error: unknown) => {
    const requestedUrl = (error as { config?: { url?: string } })?.config?.url ?? ''
    const isLoginRequest = requestedUrl.includes('/auth/login')

    if (axios.isAxiosError(error) && error.response?.status === 401 && !isLoginRequest) {
      clearAccessToken()
      window.dispatchEvent(new Event(UNAUTHORIZED_EVENT))
    }

    return Promise.reject(error)
  },
)
