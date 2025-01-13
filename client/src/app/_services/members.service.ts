import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { of, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;

  members: Member[] = [];

  constructor(private http: HttpClient) { }

  getMembers() {
    if (this.members.length > 0) return of(this.members);
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members => {
        this.members = members;
        return members;
      }),
      catchError(error => {
        console.error('Error fetching members', error);
        return throwError(() => error);
      })
    );
  }

  getMember(username: string) {
    // Check if the member is already cached
    const cachedMember = this.members.find(member => member.userName === username);
    if (cachedMember) {
      return of(cachedMember); // Return the cached member as an observable
    }
    // Fetch member from the API if not cached
    return this.http.get<Member>(this.baseUrl + 'users' + username);
    
  }
  
  updateMember(member: Member) {
    return this.http.put(`${this.baseUrl}users`, member).pipe(
      map(() => {
        // Update the cached member if it exists
        const index = this.members.indexOf(member);
        this.members[index] = member;
        
      }),
      catchError(error => {
        console.error(`Failed to update member with username: ${member.userName}`, error);
        return throwError(() => error); // Rethrow the error
      })
    );
  }
  



}
