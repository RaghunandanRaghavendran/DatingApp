import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from '../_models/user';
import { Route, Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // model: any = {};
  user: User;
  registerForm: FormGroup;
  bsDatePickerConfig: Partial<BsDatepickerConfig>;
  @Output() cancelClicked = new EventEmitter();
  constructor(
    private service: AuthService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private route: Router,
  ) {}

  ngOnInit() {
    // This is explaination for using Form Group, instead we can use a form builder
    // this.registerForm = new FormGroup({
    //   username: new FormControl('', Validators.required),
    //   password: new FormControl('', [
    //     Validators.required,
    //     Validators.minLength(4),
    //     Validators.maxLength(16)
    //   ]),
    //   confirmPassword: new FormControl('', [Validators.required])
    // }, this.passwordMatchValidator);
    this.bsDatePickerConfig = {
      containerClass: 'theme-red',
    };
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.fb.group({
    username : ['', Validators.required],
    password : ['', [Validators.required, Validators.minLength(4), Validators.maxLength(16)]],
    confirmPassword : ['', [Validators.required, Validators.minLength(4), Validators.maxLength(16)]],
    gender : ['male'],
    knownAs : ['', Validators.required],
    dateOfBirth: [null, Validators.required],
    city: ['', Validators.required],
    country: ['', Validators.required]
      }, {validators : this.passwordMatchValidator}
    );
  }

  passwordMatchValidator(match: FormGroup) {
    return match.get('password').value === match.get('confirmPassword').value ? null : {mismatch: true};
  }

  register() {
    // this.service.register(this.model).subscribe(
    //   next => {
    //     this.alertify.success('registered successfully');
    //   },
    //   error => {
    //     this.alertify.error(error);
    //   }
    // );
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);
      this.service.register(this.user).subscribe(() => {
      this.alertify.success('registered successfully');
      }, error => {
      this.alertify.error(error);
      }, () => {
      this.service.login(this.user).subscribe(() => {
        this.route.navigate(['/members']);
      });
      });
    }
  }

  cancel() {
    this.cancelClicked.emit(false);
  }
}
