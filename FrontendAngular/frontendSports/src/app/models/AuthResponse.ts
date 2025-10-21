export interface AuthResponse {
  token: string;
  userId?: number;
  userName?: string;
  role?: string;
}