import axios from 'axios'
import type { ApiResponse } from '../types/api'

export function getApiErrorMessage(error: unknown, fallback: string): string {
  if (axios.isAxiosError(error) && error.response?.data) {
    const body = error.response.data as Partial<ApiResponse<unknown>>
    if (body.message) {
      return body.message
    }
    if (body.errors?.length) {
      return body.errors.join(' ')
    }
  }

  if (error instanceof Error && error.message) {
    return error.message
  }

  return fallback
}
