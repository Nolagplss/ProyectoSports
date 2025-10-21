export interface TokenClaims {
  sub: string;                    // userId (NameIdentifier)
  email: string;                  // email
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name': string; // nombre
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': string; // rol
  permission: string | string[]; // permisos
  iat: number;
  exp: number;
}