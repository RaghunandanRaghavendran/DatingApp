import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  photoUrl: string;
  constructor(public service: AuthService, private alertify: AlertifyService, private router: Router) {}

  ngOnInit() {
    this.service.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }

  login(): void {
    this.service.login(this.model).subscribe(
      next => {
        this.alertify.success('logged in successfully');
      },
      error => {
        this.alertify.error(error);
      },
      () => {
       this.router.navigate(['/members']);
      }
    );
  }

  loggedIn() {
    return this.service.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.service.decodedtoken = null;
    this.service.currentUser = null;
    this.alertify.message('logged out successfully');
    this.router.navigate(['/home']);
  }
}
