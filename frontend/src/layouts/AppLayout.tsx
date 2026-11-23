import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '../features/auth/authContext'
import { AppRoles } from '../types/roles'

export function AppLayout() {
  const { user, logout, hasRole } = useAuth()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/login', { replace: true })
  }

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
          {hasRole(AppRoles.Manager) && <NavLink to="/manager">Manager</NavLink>}
        </nav>

        <div className="header-actions">
          <span className="header-email">{user?.email}</span>
          <button type="button" className="button button--ghost" onClick={handleLogout}>
            Log out
          </button>
        </div>
      </header>

      <main className="app-main">
        <Outlet />
      </main>
    </div>
  )
}
