import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';


const user = localStorage.getItem('user');

const httpOptions = {
  headers: new HttpHeaders({
    Authorization: user ? 'Bearer ' + JSON.parse(user).token : ''
  })
};

@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getMembers() {
    return this.http.get<Member []>(this.baseUrl + 'users', httpOptions)
  }

  getMember(username: string) {
    return this.http.get<Member>(this.baseUrl + 'users/' + username, httpOptions)
  }
}


// getMembers() {
//   return this.http.get(this.baseUrl + 'users').pipe(
//     catchError(error => {
//       console.error('Error fetching members:', error);
//       return throwError(() => error); // Handle or rethrow the error
//     })
//   );
// }
