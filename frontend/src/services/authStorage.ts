// Access tokens are kept in memory only. There is no refresh-token flow yet,
// so a page reload returns the user to /login.
let accessToken: string | null = null

export function getAccessToken(): string | null {
  return accessToken
}

export function setAccessToken(token: string): void {
  accessToken = token
}

export function clearAccessToken(): void {
  accessToken = null
}
