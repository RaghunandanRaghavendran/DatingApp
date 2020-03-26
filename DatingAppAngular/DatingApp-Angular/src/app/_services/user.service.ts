import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AlertifyService } from './alertify.service';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { User } from '../_models/user';


@Injectable({
  providedIn: 'root'
})
export class UserService {
baseUrl = environment.apiUrl;
constructor(private http: HttpClient) { }

getUsers(): Observable<User[]> {
return this.http.get<User[]>(this.baseUrl + 'users/');
}

getUser(id: number): Observable<User> {
  return this.http.get<User>(this.baseUrl + 'users/' + id);
}

updateUser(id: number, user: User) {
   return this.http.put(this.baseUrl + 'users/' + id, user);
}

setProfilePicture(userId: number, id: number) {
  return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {});
}

deletePhoto(userid: number, photoid: number) {
  return this.http.delete(this.baseUrl + 'users/' + userid + '/photos/' + photoid);
}

}
