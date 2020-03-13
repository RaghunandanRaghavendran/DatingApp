import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';

@Injectable()
export class MemberEditResolver implements Resolve<User> {
    constructor(private service: UserService, private router: Router,
                private alertify: AlertifyService, private authservice: AuthService) {}
    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        return this.service.getUser(this.authservice.decodedtoken.nameid).pipe(
            catchError(error => {
                this.alertify.error('problem retreiving your data');
                this.router.navigate(['/members']);
                return of(null);
            })
        );
    }
}
