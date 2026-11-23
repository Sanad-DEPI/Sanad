const defaultApiUrl = 'http://localhost:5045/api'

// The backend CORS policy allows the Vite dev origin by default.
export const appConfig = {
  apiBaseUrl: (import.meta.env.VITE_API_URL ?? defaultApiUrl).replace(/\/$/, ''),
}
