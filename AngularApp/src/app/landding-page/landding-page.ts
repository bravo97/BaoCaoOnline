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

  constructor(private router: Router) { }

  goToLogin() {
    this.router.navigate(['login']);
  }
}
