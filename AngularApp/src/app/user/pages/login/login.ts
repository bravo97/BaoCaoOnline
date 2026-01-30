import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LoginModel } from '../../models/loginModel';
import { CustomerVerificationResponse } from '../../models/customerVerificationModel';
import { AuthService } from '../../services/auth';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  // Login credentials
  loginModel: LoginModel = { username: '', password: '' };

  // Customer key verification
  customerKey: string = '';
  isKeyVerified: boolean = false;
  keyError: string = '';
  isVerifying: boolean = false;

  constructor(private router: Router, private auth: AuthService) { }

  ngOnInit() {
    // Check if customer key is already stored
    const storedKey = localStorage.getItem('customerKey') || sessionStorage.getItem('customerKey');
    if (storedKey) {
      this.customerKey = storedKey;
      this.isKeyVerified = true;
    }
  }

  // Verify customer key
  verifyCustomerKey() {
    // Validate input
    if (!this.customerKey.trim()) {
      this.keyError = 'Vui lòng nhập customer key';
      return;
    }

    // Validate GUID format
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    if (!guidRegex.test(this.customerKey.trim())) {
      this.keyError = 'Customer key phải là GUID hợp lệ';
      return;
    }

    this.isVerifying = true;
    this.keyError = '';

    this.auth.verifyCustomerKey(this.customerKey.trim())
      .subscribe({
        next: (response: any) => {
          console.log('Verification response:', response);

          // Parse response if it comes as a string
          let data = response;
          if (typeof response === 'string') {
            try {
              data = JSON.parse(response);
            } catch (e) {
              console.error('Error parsing response:', e);
            }
          }

          // Check for id (or Id) in the response object
          const customerId = data?.id || data?.Id;

          // Nếu API trả về response với id, nghĩa là customer key hợp lệ
          if (customerId) {
            // Key is valid - show login form
            this.isKeyVerified = true;
            this.keyError = '';

            // Store customer key (GUID) for later use
            localStorage.setItem('customerKey', customerId);
          } else {
            // Trường hợp không mong đợi - response không có id
            this.keyError = 'Customer key không hợp lệ';
            this.customerKey = '';
          }
          this.isVerifying = false;
        },
        error: (error) => {
          console.error('Key verification error:', error);

          // Handle different error scenarios
          if (error.status === 0) {
            this.keyError = 'Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng.';
          } else if (error.status === 404) {
            this.keyError = 'Customer key không tồn tại trong hệ thống.';
          } else if (error.status === 400) {
            this.keyError = error.error?.message || 'Customer key không hợp lệ.';
          } else if (error.status === 500) {
            this.keyError = 'Lỗi server. Vui lòng thử lại sau.';
          } else {
            this.keyError = error.error?.message || 'Lỗi kết nối server. Vui lòng thử lại.';
          }

          this.isVerifying = false;
        }
      });
  }

  // Handle Enter key press on customer key input
  onKeyPress(event: KeyboardEvent) {
    if (event.key === 'Enter' && !this.isVerifying) {
      this.verifyCustomerKey();
    }
  }

  // Login method
  Login() {
    // Retrieve the verified customer key from localStorage (or session fallback)
    const customerKey = localStorage.getItem('customerKey') || sessionStorage.getItem('customerKey');

    this.auth.login(this.loginModel.username, this.loginModel.password, customerKey || undefined)
      .subscribe({
        next: () => {
          // login thành công => điều hướng
          this.router.navigate(['home']);
        },
        error: () => {
          alert('Tên đăng nhập hoặc mật khẩu không chính xác!');
        }
      });
  }
}
