import { Component, ViewEncapsulation } from '@angular/core';
import { Sidebar } from "../../layout/sidebar/sidebar";
import { Header } from "../../layout/header/header";
import { FeedbackModel, MessageModel, Feedback } from '../../models/feedbackModel';
import { CommonModule } from '@angular/common';
import { FormsModule } from "@angular/forms";
import { AdminFeedbackService } from '../../services/admin-feedback.service';
import { ToastrNotification } from '../../../shared/services/toastr-service';

@Component({
  selector: 'app-feedback',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './feedback.html',
  styleUrl: './feedback.scss'
})
export class FeedbackComponent {
  selectedCustomer?: FeedbackModel;
  replyText: string = '';
  searchText: string = '';
  feedbacks: FeedbackModel[] = [];
  rawFeedbacks: Feedback[] = [];
  isLoading = false;

  constructor(private service: AdminFeedbackService, private toastr: ToastrNotification) { }

  ngOnInit() {
    this.loadFeedbacks();
  }

  loadFeedbacks() {
    this.isLoading = true;
    this.service.getAll().subscribe({
      next: (res) => {
        if (res.success) {
          this.rawFeedbacks = res.data;
          this.feedbacks = this.rawFeedbacks.map(f => this.mapToViewModel(f));
          // Reselect if exists
          if (this.selectedCustomer) {
            const updated = this.feedbacks.find(f => f.customerId === this.selectedCustomer?.customerId); // Using ID as CustomerID placeholder for now or mapping correctly
            if (updated) this.selectedCustomer = updated;
          }
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.toastr.error('Không thể tải danh sách góp ý');
        this.isLoading = false;
      }
    });
  }

  mapToViewModel(f: Feedback): FeedbackModel {
    const messages: MessageModel[] = [];
    // User Message
    messages.push({
      from: 'customer',
      content: f.message,
      time: new Date(f.createdAt).toLocaleString('vi-VN')
    });
    // Admin Response
    if (f.response) {
      messages.push({
        from: 'admin',
        content: f.response,
        time: f.responseAt ? new Date(f.responseAt).toLocaleString('vi-VN') : ''
      });
    }

    return {
      customerId: f.id, // Using FeedbackID as the identifier for selection
      customerName: f.subject, // Using Subject as Name for now, or fetch user details
      status: f.status === 2 ? 'Resolved' : 'Pending',
      lastMessage: f.response || f.message,
      messages: messages
    };
  }

  get filteredFeedbacks() {
    return this.feedbacks.filter(f =>
      f.customerName.toLowerCase().includes(this.searchText.toLowerCase()) ||
      f.lastMessage.toLowerCase().includes(this.searchText.toLowerCase())
    );
  }

  isMobileChatActive: boolean = false;

  selectCustomer(f: FeedbackModel) {
    this.selectedCustomer = f;
    this.isMobileChatActive = true;
  }

  backToList() {
    this.isMobileChatActive = false;
  }

  sendReply() {
    if (!this.replyText.trim() || !this.selectedCustomer) return;

    const feedbackId = this.selectedCustomer.customerId; // This is actually the Feedback ID mapped

    this.service.respond(feedbackId, this.replyText.trim()).subscribe({
      next: () => {
        this.toastr.success('Phản hồi thành công');
        this.replyText = '';
        this.loadFeedbacks(); // Reload to refresh state
      },
      error: (err) => {
        console.error(err);
        this.toastr.error('Gửi phản hồi thất bại');
      }
    });
  }

}
