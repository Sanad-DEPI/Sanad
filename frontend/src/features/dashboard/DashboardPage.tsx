import { useEffect, useState } from 'react'
import { Alert } from '../../components/Alert'
import { LoadingIndicator } from '../../components/LoadingIndicator'
import { getApiErrorMessage } from '../../services/errors'
import { getCurrentUser } from '../../services/userService'
import type { CurrentUser } from '../../types/auth'
import { useAuth } from '../auth/authContext'

export function DashboardPage() {
  const { user } = useAuth()
  const [currentUser, setCurrentUser] = useState<CurrentUser | null>(null)
  const [error, setError] = useState('')
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    let active = true

    getCurrentUser()
      .then((result) => {
        if (active) {
          setCurrentUser(result)
        }
      })
      .catch((reason: unknown) => {
        if (active) {
          setError(getApiErrorMessage(reason, 'Could not load your profile.'))
        }
      })
      .finally(() => {
        if (active) {
          setIsLoading(false)
        }
      })

    return () => {
      active = false
    }
  }, [])

  return (
    <section className="page">
      <div className="page-heading">
        <h1>Dashboard</h1>
        <p className="muted">Signed in as {user?.email}</p>
      </div>

      {error && <Alert variant="error">{error}</Alert>}

      <div className="card">
        <h2>Your account</h2>
        {isLoading ? (
          <LoadingIndicator label="Loading your profile…" />
        ) : currentUser ? (
          <dl className="details">
            <div>
              <dt>Email</dt>
              <dd>{currentUser.email}</dd>
            </div>
            <div>
              <dt>User ID</dt>
              <dd className="monospace">{currentUser.id}</dd>
            </div>
            <div>
              <dt>Roles</dt>
              <dd>
                <div className="chips">
                  {currentUser.roles.length > 0 ? (
                    currentUser.roles.map((role) => (
                      <span className="chip" key={role}>
                        {role}
                      </span>
                    ))
                  ) : (
                    <span className="muted">No roles assigned</span>
                  )}
                </div>
              </dd>
            </div>
          </dl>
        ) : (
          <p className="muted">No profile data available.</p>
        )}
      </div>
    </section>
  )
}
