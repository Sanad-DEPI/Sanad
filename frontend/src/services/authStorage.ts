// The backend supports refresh-token rotation, but this client does not yet
// store or use refresh tokens, so a page reload returns the user to /login.
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
