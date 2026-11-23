import { screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { api } from '../../services/api'
import { clearAccessToken } from '../../services/authStorage'
import { axiosResponse, renderApp } from '../../test/test-utils'

vi.mock('../../services/api', () => ({
  UNAUTHORIZED_EVENT: 'sanad:unauthorized',
  api: {
    post: vi.fn(),
    get: vi.fn(),
  },
}))

const loginBody = {
  succeeded: true,
  data: {
    token: 'test-token',
    expiresAtUtc: '2026-11-24T00:00:00Z',
    email: 'manager@example.test',
    roles: ['Manager'],
  },
  message: null,
  errors: [],
}

describe('LoginPage', () => {
  beforeEach(() => {
    clearAccessToken()
    vi.clearAllMocks()
  })

  it('shows validation errors and does not submit an invalid form', async () => {
    const user = userEvent.setup()
    renderApp(['/login'])

    await user.click(screen.getByRole('button', { name: /sign in/i }))

    expect(await screen.findByText('Email is required.')).toBeInTheDocument()
    expect(screen.getByText('Password is required.')).toBeInTheDocument()
    expect(api.post).not.toHaveBeenCalled()
  })

  it('shows the API error when credentials are rejected', async () => {
    vi.mocked(api.post).mockRejectedValueOnce(new Error('Invalid email or password.'))
    const user = userEvent.setup()
    renderApp(['/login'])

    await user.type(screen.getByLabelText(/email/i), 'user@example.test')
    await user.type(screen.getByLabelText(/password/i), 'Wrong@12345')
    await user.click(screen.getByRole('button', { name: /sign in/i }))

    expect(await screen.findByRole('alert')).toHaveTextContent('Invalid email or password.')
    expect(screen.getByRole('heading', { name: /sign in/i })).toBeInTheDocument()
  })

  it('authenticates, stores the token and returns to the requested manager route', async () => {
    vi.mocked(api.post).mockResolvedValueOnce(axiosResponse(loginBody))
    vi.mocked(api.get).mockResolvedValueOnce(
      axiosResponse({
        succeeded: true,
        data: 'Manager access granted.',
        message: null,
        errors: [],
      }),
    )
    const user = userEvent.setup()
    renderApp(['/manager'])

    expect(await screen.findByRole('heading', { name: /sign in/i })).toBeInTheDocument()
    await user.type(screen.getByLabelText(/email/i), 'manager@example.test')
    await user.type(screen.getByLabelText(/password/i), 'Test@12345')
    await user.click(screen.getByRole('button', { name: /sign in/i }))

    expect(await screen.findByText('Manager access granted.')).toBeInTheDocument()
    expect(api.post).toHaveBeenCalledWith('/auth/login', {
      email: 'manager@example.test',
      password: 'Test@12345',
    })
    expect(api.get).toHaveBeenCalledWith('/test/manager')
  })
})
