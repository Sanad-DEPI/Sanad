import { NavLink, Outlet } from 'react-router-dom'
import { useAuth } from '../features/auth/authContext'

export function AppLayout() {
  const { user } = useAuth()

  return (
    <div className="app-shell">
      <header className="app-header">
        <div className="brand">
          <span className="brand-mark">S</span>
          <span>SANAD</span>
        </div>

        <nav className="app-nav" aria-label="Main navigation">
          <NavLink to="/" end>
            Dashboard
          </NavLink>
          <NavLink to="/manager">Manager</NavLink>
        </nav>

        <div className="header-actions">
          <span className="header-email">{user?.email ?? 'Guest'}</span>
        </div>
      </header>

      <main className="app-main">
        <Outlet />
      </main>
    </div>
  )
}
