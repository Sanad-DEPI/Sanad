interface FormFieldProps {
  id: string
  label: string
  value: string
  onChange: (value: string) => void
  error?: string
  type?: 'text' | 'email' | 'password'
  autoComplete?: string
  required?: boolean
}

export function FormField({
  id,
  label,
  value,
  onChange,
  error,
  type = 'text',
  autoComplete,
  required = false,
}: FormFieldProps) {
  return (
    <div className="form-field">
      <label htmlFor={id}>{label}</label>
      <input
        id={id}
        name={id}
        type={type}
        value={value}
        onChange={(event) => onChange(event.target.value)}
        autoComplete={autoComplete}
        aria-invalid={Boolean(error)}
        aria-describedby={error ? `${id}-error` : undefined}
        required={required}
      />
      {error && (
        <span className="field-error" id={`${id}-error`}>
          {error}
        </span>
      )}
    </div>
  )
}
