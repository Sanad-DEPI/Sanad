import { screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { api } from '../services/api'
import { clearAccessToken } from '../services/authStorage'
import { axiosResponse, renderApp } from '../test/test-utils'

vi.mock('../services/api', () => ({
  UNAUTHORIZED_EVENT: 'sanad:unauthorized',
  api: {
    post: vi.fn(),
    get: vi.fn(),
  },
}))

function loginBody(roles: string[]) {
  return {
    succeeded: true,
    data: {
      token: 'test-token',
      expiresAtUtc: '2026-11-24T00:00:00Z',
      email: roles.includes('Manager') ? 'manager@example.test' : 'donor@example.test',
      roles,
    },
    message: null,
    errors: [],
  }
}

describe('protected routing', () => {
  beforeEach(() => {
    clearAccessToken()
    vi.clearAllMocks()
  })

  it('redirects an unauthenticated user from a protected route to login', async () => {
    renderApp(['/'])

    expect(await screen.findByRole('heading', { name: /sign in/i })).toBeInTheDocument()
    expect(api.get).not.toHaveBeenCalled()
  })

  it('denies a donor access to the manager route', async () => {
    vi.mocked(api.post).mockResolvedValueOnce(axiosResponse(loginBody(['Donor'])))
    const user = userEvent.setup()
    renderApp(['/manager'])

    await user.type(screen.getByLabelText(/email/i), 'donor@example.test')
    await user.type(screen.getByLabelText(/password/i), 'Test@12345')
    await user.click(screen.getByRole('button', { name: /sign in/i }))

    expect(await screen.findByRole('heading', { name: /access denied/i })).toBeInTheDocument()
    expect(api.get).not.toHaveBeenCalled()
  })

  it('logs out and returns to the login page', async () => {
    vi.mocked(api.post).mockResolvedValueOnce(axiosResponse(loginBody(['Manager'])))
    vi.mocked(api.get).mockResolvedValueOnce(
      axiosResponse({
        succeeded: true,
        data: {
          id: 'user-1',
          email: 'manager@example.test',
          roles: ['Manager'],
        },
        message: null,
        errors: [],
      }),
    )
    const user = userEvent.setup()
    renderApp(['/'])

    await user.type(screen.getByLabelText(/email/i), 'manager@example.test')
    await user.type(screen.getByLabelText(/password/i), 'Test@12345')
    await user.click(screen.getByRole('button', { name: /sign in/i }))

    expect(await screen.findByRole('heading', { name: 'Dashboard' })).toBeInTheDocument()
    await user.click(screen.getByRole('button', { name: /log out/i }))

    expect(await screen.findByRole('heading', { name: /sign in/i })).toBeInTheDocument()
  })
})
