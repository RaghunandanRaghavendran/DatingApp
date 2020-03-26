import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { error } from 'util';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
user: User;
photoUrl: string;
@ViewChild('editForm', {static: true}) editForm: NgForm;

@HostListener('window:beforeunload', ['$event'])
unloadNotification($event: any) {
$event.returnValue = true;
}
  constructor(private route: ActivatedRoute, private alertify: AlertifyService,
              private service: UserService, private authservice: AuthService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      // tslint:disable-next-line: no-string-literal
      this.user = data['user'];
    });
    this.authservice.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }
  UpdateUser() {
    this.service.updateUser(this.authservice.decodedtoken.nameid, this.user).subscribe(next => {
        this.alertify.success('profile updated success');
        this.editForm.reset(this.user);
      }, err => {
        this.alertify.error(err);
      }
    );
  }

  updatePhoto(photoURL: string) {
    this.user.photoUrl = photoURL;
  }

}
