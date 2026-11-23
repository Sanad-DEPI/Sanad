import { useState, type FormEvent } from 'react'
import { Navigate, useLocation, useNavigate } from 'react-router-dom'
import { Alert } from '../../components/Alert'
import { FormField } from '../../components/FormField'
import { getApiErrorMessage } from '../../services/errors'
import { useAuth } from './authContext'

interface FieldErrors {
  email?: string
  password?: string
}

const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

export function LoginPage() {
  const { isAuthenticated, login } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()

  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({})
  const [submitError, setSubmitError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  const from = (location.state as { from?: { pathname?: string } } | null)?.from?.pathname ?? '/'

  if (isAuthenticated) {
    return <Navigate to={from} replace />
  }

  const validate = (): FieldErrors => {
    const errors: FieldErrors = {}
    if (!email.trim()) {
      errors.email = 'Email is required.'
    } else if (!emailPattern.test(email.trim())) {
      errors.email = 'Enter a valid email address.'
    }
    if (!password) {
      errors.password = 'Password is required.'
    }
    return errors
  }

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setSubmitError('')

    const errors = validate()
    setFieldErrors(errors)
    if (Object.keys(errors).length > 0) {
      return
    }

    setIsSubmitting(true)
    try {
      await login(email.trim(), password)
      navigate(from, { replace: true })
    } catch (error: unknown) {
      setSubmitError(getApiErrorMessage(error, 'Login failed. Please try again.'))
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="login-page">
      <section className="login-card" aria-labelledby="login-heading">
        <div className="login-brand">
          <span className="brand-mark">S</span>
          <span>SANAD</span>
        </div>
        <h1 id="login-heading">Sign in</h1>
        <p className="muted">Access the SANAD management platform.</p>

        {submitError && <Alert variant="error">{submitError}</Alert>}

        <form onSubmit={handleSubmit} noValidate>
          <FormField
            id="email"
            label="Email"
            type="email"
            value={email}
            onChange={setEmail}
            error={fieldErrors.email}
            autoComplete="username"
            required
          />
          <FormField
            id="password"
            label="Password"
            type="password"
            value={password}
            onChange={setPassword}
            error={fieldErrors.password}
            autoComplete="current-password"
            required
          />
          <button type="submit" className="button button--primary button--full" disabled={isSubmitting}>
            {isSubmitting ? 'Signing in…' : 'Sign in'}
          </button>
        </form>
      </section>
    </div>
  )
}
