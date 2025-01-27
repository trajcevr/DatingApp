import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { User } from '../_models/user';
import { ReplaySubject } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  // Add this method to get current user synchronously
  getCurrentUser(): User {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }

  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          localStorage.setItem('user', JSON.stringify(user)); 
          this.setCurrentUser(user);
        }
      })
    )
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map((user: User) => {
        if (user) {
          localStorage.setItem('user', JSON.stringify(user)); 
          this.setCurrentUser(user);
        }
        return user;
      })
    )
  }
  
  setCurrentUser(user: User) {
    if (!user) {
      console.error('User is null or undefined');
      return;
    }
  
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
  
    if (Array.isArray(roles)) {
      user.roles = roles;
    } else {
      user.roles.push(roles);
    }
  
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }
  

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }

  getDecodedToken(token) {
    return JSON.parse(atob(token.split('.')[1]));

  }
}