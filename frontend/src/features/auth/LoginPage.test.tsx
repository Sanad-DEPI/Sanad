import { screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import { renderApp } from '../../test/test-utils'

describe('LoginPage', () => {
  it('is not routed; home renders the template page', async () => {
    renderApp(['/login'])

    expect(await screen.findByRole('heading', { name: 'كل تبرع يصل إلى من يحتاجه' })).toBeInTheDocument()
  })
})
