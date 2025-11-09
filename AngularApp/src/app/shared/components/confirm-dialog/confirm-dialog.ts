import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-confirm-dialog',
  imports: [CommonModule],
  templateUrl: './confirm-dialog.html',
  styleUrl: './confirm-dialog.scss',
})
export class ConfirmDialog {
  @Input() visible = false;
  @Input() title = 'Xác nhận';
  @Input() message = 'Bạn có chắc muốn xóa?';
  @Output() confirmed = new EventEmitter<boolean>();

  confirm() {
    this.confirmed.emit(true);
    this.visible = false;
  }

  cancel() {
    this.confirmed.emit(false);
    this.visible = false;
  }
}
