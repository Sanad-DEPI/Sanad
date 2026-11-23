// Keep these values aligned with backend/src/SANAD.Domain/Constants/AppRoles.cs.
export const AppRoles = {
  Manager: 'Manager',
  PayoutAuditor: 'PayoutAuditor',
  Treasurer: 'Treasurer',
  DocumentReviewer: 'DocumentReviewer',
  FieldOfficer: 'FieldOfficer',
  Donor: 'Donor',
} as const

export type AppRole = (typeof AppRoles)[keyof typeof AppRoles]
