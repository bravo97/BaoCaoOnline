import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FeedbackService } from '../../services/feedback.service';
import { ToastrNotification } from '../../../shared/services/toastr-service';

@Component({
    selector: 'app-feedback-dialog',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './feedback-dialog.component.html',
    styleUrl: './feedback-dialog.component.scss'
})
export class FeedbackDialog {
    @Output() close = new EventEmitter<void>();
    visible = false;

    subject = '';
    message = '';
    isSubmitting = false;

    constructor(
        private feedbackService: FeedbackService,
        private notify: ToastrNotification
    ) { }

    open() {
        this.subject = '';
        this.message = '';
        this.visible = true;
    }

    closeDialog() {
        this.visible = false;
        this.close.emit();
    }

    submit() {
        if (!this.subject || !this.message) {
            this.notify.warning('Vui lòng nhập đầy đủ tiêu đề và nội dung');
            return;
        }

        this.isSubmitting = true;
        const dto = {
            subject: this.subject,
            message: this.message
            // UserEmail and CustomerId will be handled by backend or auth context if needed, 
            // but DTO allows sending them. For now let's rely on what the API needs.
            // API DTO: CustomerId, UserEmail, Subject, Message.
            // Ideally we should extract email from token if possible, or let user input it?
            // For simplicity/requirement "nhập thông tin phản ánh", let's assume authenticated user context handles identification if possible, 
            // OR we add email field if it's not auto-extracted.
            // Looking at Controller: 
            // var customerId = User.Identity?.Name; (Not used in Create action explicitly to override dto?)
            // Controller uses [FromBody] CreateFeedbackDto. It doesn't auto-set from User.Identity in Create method shown previously.
            // So we might need to pass it if we want it linked.
            // However, usually `User.Identity.Name` is CustomerId in this system.
        };

        // Let's add simple fields for now. 
        // If backend doesn't auto-fill, we might send them if available in localStorage/SessionStorage
        // But for quick "Góp ý", Subject/Message is key.

        this.feedbackService.create(dto).subscribe({
            next: () => {
                this.notify.success('Cảm ơn bạn đã đóng góp ý kiến!');
                this.isSubmitting = false;
                this.closeDialog();
            },
            error: () => {
                this.notify.error('Có lỗi xảy ra, vui lòng thử lại sau.');
                this.isSubmitting = false;
            }
        });
    }
}
