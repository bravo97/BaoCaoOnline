export interface AccountModel {
  id: string;
  customerId: string;
  username: string;
  password: string;
  role: 'Admin' | 'Regular';
  note?: string;
}