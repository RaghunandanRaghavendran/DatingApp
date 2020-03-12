import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';

@Component({
  selector: 'app-member-lists',
  templateUrl: './member-lists.component.html',
  styleUrls: ['./member-lists.component.css']
})
export class MemberListsComponent implements OnInit {
  users: User[];
  constructor(
    private service: UserService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.service.getUsers().subscribe((userArray: User[]) => {
      this.users = userArray;
    }, error => {
      this.alertify.error(error);
    });
  }
}
