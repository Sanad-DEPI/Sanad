import { AxiosError } from 'axios'
import { describe, expect, it } from 'vitest'
import { getApiErrorMessage } from './errors'

describe('getApiErrorMessage', () => {
  it('uses the backend envelope message when present', () => {
    const error = new AxiosError('Request failed')
    error.response = {
      data: {
        succeeded: false,
        data: null,
        message: 'Invalid email or password.',
        errors: [],
      },
    } as never

    expect(getApiErrorMessage(error, 'Fallback')).toBe('Invalid email or password.')
  })

  it('falls back for unknown error shapes', () => {
    expect(getApiErrorMessage(new Error('Network failure'), 'Fallback')).toBe('Network failure')
    expect(getApiErrorMessage(null, 'Fallback')).toBe('Fallback')
  })
})
