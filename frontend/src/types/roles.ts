// Keep these values aligned with backend/src/SANAD.Domain/Constants/AppRoles.cs.
export const AppRoles = {
  Beneficiary: 'Beneficiary',
  Donor: 'Donor',
  SuperAdmin: 'SuperAdmin',
  Manager: 'Manager',
  DocumentReviewer: 'DocumentReviewer',
  FieldOfficer: 'FieldOfficer',
  PayoutAuditor: 'PayoutAuditor',
  Treasurer: 'Treasurer',
} as const

export type AppRole = (typeof AppRoles)[keyof typeof AppRoles]
