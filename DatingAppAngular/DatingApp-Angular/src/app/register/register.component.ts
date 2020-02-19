import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model: any = {};
  @Output() cancelClicked = new EventEmitter();
  constructor(private service: AuthService) {}

  ngOnInit() {}
  register() {
    this.service.register(this.model).subscribe(
      next => {
        console.log('registered successfully');
      },
      error => {
        console.log('error occured');
      }
    );
  }

  cancel() {
    this.cancelClicked.emit(false);
    console.log('Cancelled');
  }
}
