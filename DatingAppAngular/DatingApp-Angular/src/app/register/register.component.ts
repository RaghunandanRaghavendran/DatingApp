import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model: any = {};
  @Output() cancelClicked = new EventEmitter();
  constructor(private service: AuthService, private alertify: AlertifyService) {}

  ngOnInit() {}
  register() {
    this.service.register(this.model).subscribe(
      next => {
        this.alertify.success('registered successfully');
      },
      error => {
        this.alertify.error(error);
      }
    );
  }

  cancel() {
    this.cancelClicked.emit(false);
  }
}
