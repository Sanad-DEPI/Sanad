export interface LoginResponse {
  token: string
  expiresAtUtc: string
  email: string
  roles: string[]
}

export interface AuthenticatedUser {
  email: string
  roles: string[]
  expiresAtUtc: string
}

export interface CurrentUser {
  id: string
  email: string
  roles: string[]
}
