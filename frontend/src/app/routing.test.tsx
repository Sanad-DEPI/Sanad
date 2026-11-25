import { screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import { renderApp } from '../test/test-utils'

describe('routing', () => {
  it('renders the template home page on root', async () => {
    renderApp(['/'])

    expect(await screen.findByRole('heading', { name: 'كل تبرع يصل إلى من يحتاجه' })).toBeInTheDocument()
  })

  it('renders manager route without login', async () => {
    renderApp(['/manager'])

    expect(await screen.findByRole('heading', { name: /manager area/i })).toBeInTheDocument()
  })
})
