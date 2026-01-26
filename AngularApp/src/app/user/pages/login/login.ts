import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LoginModel } from '../../models/loginModel';
import { AuthService } from '../../services/auth';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone:true,
  imports: [FormsModule,CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  loginModel: LoginModel = {username: '', password: '' };
  constructor(private router: Router,private auth:AuthService) {}
  Login() {
      this.auth.login('5a95911c-10a0-4777-a468-a1af22977f41',this.loginModel.username, this.loginModel.password)
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
