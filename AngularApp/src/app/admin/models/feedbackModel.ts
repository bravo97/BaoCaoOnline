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