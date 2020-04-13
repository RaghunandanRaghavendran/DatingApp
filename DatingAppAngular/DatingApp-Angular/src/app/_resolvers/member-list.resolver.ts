import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class MemberListsResolver implements Resolve<User[]> {
    pageSize = 5;
    pageNumber = 1;
    constructor(private service: UserService, private router: Router,
                private alertify: AlertifyService) {}
    resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
        return this.service.getUsers(this.pageNumber, this.pageSize).pipe(
            catchError(error => {
                this.alertify.error('problem retreiving the data');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
