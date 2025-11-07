import { Component, ViewEncapsulation } from '@angular/core';
import { Sidebar } from "../../layout/sidebar/sidebar";
import { Header } from "../../layout/header/header";
import { FeedbackModel, MessageModel } from '../../models/feedbackModel';
import { CommonModule } from '@angular/common';
import { FormsModule } from "@angular/forms";

@Component({
  selector: 'app-feedback',
  standalone:true,
  imports: [CommonModule, FormsModule],
  templateUrl: './feedback.html',
  styleUrl: './feedback.scss'
})
export class Feedback {
  selectedCustomer?: FeedbackModel;
  replyText: string = '';
  feedbacks: FeedbackModel[] = [
    {
      customerId: 'C001',
      customerName: 'Nguyễn Văn A',
      status: 'Pending',
      lastMessage: 'App bị treo khi đăng nhập.',
      messages: [
        { from: 'customer', content: 'App bị treo khi đăng nhập.', time: '2025-11-06 10:02' }
      ]
    },
    {
      customerId: 'C002',
      customerName: 'Trần Thị B',
      status: 'Resolved',
      lastMessage: 'Cảm ơn bạn, tôi đã khắc phục được rồi.',
      messages: [
        { from: 'customer', content: 'Không cập nhật được thông tin cá nhân.', time: '2025-11-06 09:00' },
        { from: 'admin', content: 'Chị vui lòng thử đăng xuất rồi đăng nhập lại nhé.', time: '2025-11-06 09:10' },
        { from: 'customer', content: 'Cảm ơn bạn, tôi đã khắc phục được rồi.', time: '2025-11-06 09:30' }
      ]
    },
    {
      customerId: 'C003',
      customerName: 'Lê Văn C',
      status: 'Pending',
      lastMessage: 'Tôi muốn góp ý về giao diện mới.',
      messages: [
        { from: 'customer', content: 'Tôi muốn góp ý về giao diện mới.', time: '2025-11-06 08:45' }
      ]
    }
  ];

  
  selectCustomer(f: FeedbackModel) {
    this.selectedCustomer = f;
  }

  sendReply() {
    if (!this.replyText.trim() || !this.selectedCustomer) return;

    const newMsg: MessageModel = {
      from: 'admin',
      content: this.replyText.trim(),
      time: new Date().toLocaleString('vi-VN')
    };
    this.selectedCustomer.messages.push(newMsg);
    this.selectedCustomer.lastMessage = this.replyText.trim();
    this.selectedCustomer.status = 'Resolved';
    this.replyText = '';
  }
  
}
