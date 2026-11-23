import { useEffect, useState } from 'react'
import { Alert } from '../../components/Alert'
import { LoadingIndicator } from '../../components/LoadingIndicator'
import { getApiErrorMessage } from '../../services/errors'
import { getManagerAccess } from '../../services/userService'

export function ManagerPage() {
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    let active = true

    getManagerAccess()
      .then((result) => {
        if (active) {
          setMessage(result)
        }
      })
      .catch((reason: unknown) => {
        if (active) {
          setError(getApiErrorMessage(reason, 'Manager access check failed.'))
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
        <h1>Manager area</h1>
        <p className="muted">This page is protected by both the frontend route guard and the API.</p>
      </div>

      <div className="card">
        {isLoading ? (
          <LoadingIndicator label="Checking manager access…" />
        ) : error ? (
          <Alert variant="error">{error}</Alert>
        ) : (
          <Alert variant="success">{message}</Alert>
        )}
      </div>
    </section>
  )
}
