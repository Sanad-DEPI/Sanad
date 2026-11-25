interface ProgressProps {
  value: number
  max: number
  label: string
  caption?: string
}

export function Progress({ value, max, label, caption }: ProgressProps) {
  const percent = max > 0 ? Math.min(100, Math.round((value / max) * 100)) : 0

  return (
    <div>
      <div
        className="progress"
        role="progressbar"
        aria-valuenow={value}
        aria-valuemin={0}
        aria-valuemax={max}
        aria-label={label}
      >
        <div className="progress__bar" style={{ width: `${percent}%` }} />
      </div>
      <div className="progress-meta">
        <span>{caption}</span>
        <span>{percent.toLocaleString('ar-EG')}٪</span>
      </div>
    </div>
  )
}
