import type { ReactNode } from 'react'

export type BadgeVariant = 'primary' | 'secondary' | 'accent'

interface CardProps {
  title?: string
  badge?: string
  badgeVariant?: BadgeVariant
  className?: string
  children: ReactNode
}

export function Card({ title, badge, badgeVariant = 'primary', className, children }: CardProps) {
  const classes = ['card', 'card--stack', className].filter(Boolean).join(' ')
  const badgeClasses = badgeVariant === 'primary' ? 'chip' : `chip chip--${badgeVariant}`

  return (
    <article className={classes}>
      {badge && <span className={badgeClasses}>{badge}</span>}
      {title && <h3 className="card__title">{title}</h3>}
      {children}
    </article>
  )
}
