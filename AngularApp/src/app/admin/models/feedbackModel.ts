// Helper for View
export interface MessageModel {
  from: 'customer' | 'admin';
  content: string;
  time: string;
}

export interface FeedbackModel {
  customerId: string;
  customerName: string;
  status: 'Pending' | 'Resolved';
  lastMessage: string;
  messages: MessageModel[];
}

// Backend Entity
export interface Feedback {
  id: string;
  customerId?: string;
  userEmail?: string;
  subject: string;
  message: string;
  status: 0 | 1 | 2; // New, Open, Closed
  response?: string;
  createdAt: string;
  responseAt?: string;
}

// Enum Helper
export enum FeedbackStatus {
  New = 0,
  Open = 1,
  Closed = 2
}
