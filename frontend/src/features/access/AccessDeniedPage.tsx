import { Link } from 'react-router-dom'

export function AccessDeniedPage() {
  return (
    <section className="page">
      <div className="card card--centered">
        <h1>Access denied</h1>
        <p className="muted">Your account does not have permission to view this page.</p>
        <Link className="button button--primary" to="/">
          Back to dashboard
        </Link>
      </div>
    </section>
  )
}
