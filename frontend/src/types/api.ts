export interface ApiResponse<T> {
  succeeded: boolean
  data: T | null
  message: string | null
  errors: string[]
}
