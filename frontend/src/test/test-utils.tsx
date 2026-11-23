import { render } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import App from '../app/App'
import { AuthProvider } from '../features/auth/AuthProvider'

export function renderApp(initialEntries: string[] = ['/']) {
  return render(
    <MemoryRouter initialEntries={initialEntries}>
      <AuthProvider>
        <App />
      </AuthProvider>
    </MemoryRouter>,
  )
}

export function axiosResponse<T>(data: T) {
  return {
    data,
    status: 200,
    statusText: 'OK',
    headers: {},
    config: { headers: {} },
  } as never
}
