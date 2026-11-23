import type { ReactNode } from 'react'

type AlertVariant = 'error' | 'success' | 'info'

export function Alert({
  variant = 'info',
  children,
}: {
  variant?: AlertVariant
  children: ReactNode
}) {
  return (
    <div className={`alert alert--${variant}`} role={variant === 'error' ? 'alert' : 'status'}>
      {children}
    </div>
  )
}
