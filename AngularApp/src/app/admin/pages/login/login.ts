import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LoginModel } from '../../models/loginModel';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  imports: [CommonModule,FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class LoginAdmin {
  loginModel: LoginModel = { username: '', password: '' };

  constructor(private router: Router, private auth: AuthService) {}

  onSubmit() {
    this.auth.login(this.loginModel.username, this.loginModel.password)
    .subscribe({
      next: () => {
        // login thành công => điều hướng
        this.router.navigate(['/admin']);
      },
      error: () => {
        alert('Tên đăng nhập hoặc mật khẩu không chính xác!');
      }
    });
  }


  onForgotPassword() {
    alert('Chức năng khôi phục mật khẩu đang được phát triển.');
  }
}
