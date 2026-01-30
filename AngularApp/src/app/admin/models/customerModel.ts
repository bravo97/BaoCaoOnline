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
  sqlLogin: string;
  sqlReport: string;
  sqlColumnQuery: string;
  sqlParameter: string;
  note?: string;
}