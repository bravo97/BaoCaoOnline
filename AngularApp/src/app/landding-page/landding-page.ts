import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-landding-page',
  imports: [],
  templateUrl: './landding-page.html',
  styleUrl: './landding-page.scss',
})
export class LanddingPage {
title = 'landing-page-bao-cao-online';

  // Dữ liệu cho phần tính năng (Features Section)
  features = [
    { 
      icon: '⚡', 
      title: 'Dữ Liệu Tức Thời', 
      description: 'Cập nhật từng giây, loại bỏ độ trễ thông tin để ra quyết định nhanh chóng.' 
    },
    { 
      icon: '🔒', 
      title: 'Bảo Mật Tuyệt Đối', 
      description: 'Hệ thống mã hóa end-to-end, đảm bảo dữ liệu kinh doanh của bạn luôn an toàn.' 
    },
    { 
      icon: '📱', 
      title: 'Truy Cập Mọi Nơi', 
      description: 'Xem báo cáo trên mọi thiết bị: di động, máy tính bảng, và desktop.' 
    },
    { 
      icon: '💡', 
      title: 'Tùy Biến Linh Hoạt', 
      description: 'Dễ dàng tùy chỉnh giao diện, bộ lọc và loại biểu đồ theo nhu cầu riêng.' 
    }
  ];

  // Hàm xử lý sự kiện khi người dùng click vào CTA
  openSignup() {
    alert('Cảm ơn bạn đã quan tâm! Chức năng chuyển hướng/mở form đăng ký sẽ được kích hoạt tại đây.');
    // Logic thực tế:
    // 1. Chuyển hướng: this.router.navigate(['/signup']);
    // 2. Mở Modal: this.modalService.open(SignupFormComponent);
  }

  constructor(private router: Router) {}

  goToLogin() {
    this.router.navigate(['login']);
  }
}
