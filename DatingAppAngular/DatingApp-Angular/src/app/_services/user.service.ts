import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { AlertifyService } from './alertify.service';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { Pagination, PaginationResult } from '../_models/Pagination';
import { map } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class UserService {
baseUrl = environment.apiUrl;
constructor(private http: HttpClient) { }

getUsers(page?, itemsPerPage?, userParams?): Observable<PaginationResult<User[]>> {
  const paginationResult: PaginationResult<User[]> = new PaginationResult<User[]>();

  let params = new HttpParams();
  if (page != null && itemsPerPage != null) {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);
  }

  if (userParams != null) {
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);
  }
   // Change the result based on pagination
  // return this.http.get<User[]>(this.baseUrl + 'users/');

  return this.http.get<User[]>(this.baseUrl + 'users/', {observe : 'response', params}).pipe(
    map(response => {
      paginationResult.result = response.body;
      if (response.headers.get('Pagination') != null) {
        paginationResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }
      return paginationResult;
    })
  );
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
