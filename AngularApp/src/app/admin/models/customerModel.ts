export interface CustomerModel {
  id: string;
  name: string;
  email: string;
  ipAddress: string;
  port: number;
  serverName: string;
  userName: string;
  password: string;
  databaseName: string;
  sqlReport:string;
  note?: string;
}