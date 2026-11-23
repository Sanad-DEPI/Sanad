export function LoadingIndicator({ label = 'Loading…' }: { label?: string }) {
  return (
    <p className="muted" role="status">
      {label}
    </p>
  )
}
