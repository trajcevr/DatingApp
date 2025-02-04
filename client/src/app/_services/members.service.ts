import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { of, throwError } from 'rxjs';
import { catchError, map, take } from 'rxjs/operators';
import { PaginatedResult } from '../_models/pagination';
import { JsonPipe } from '@angular/common';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  user: User;
  userParams: UserParams;

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
    this.user = user;
    this.userParams = new UserParams(user);

    })
   }

  getUserParams() {
    return this.userParams
  }

  setUserParams(params: UserParams) {
    this.userParams = params
  }

  resetUserParams() {
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  getMembers(userParams: UserParams) {
    var response = this.memberCache.get(Object.values(userParams).join('-'));

    if (response) {
      return of(response);
    }

    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
  
    // Only append minAge and maxAge if they are defined
    if (userParams.minAge !== undefined) {
      params = params.append('minAge', userParams.minAge.toString());
    }
  
    if (userParams.maxAge !== undefined) {
      params = params.append('maxAge', userParams.maxAge.toString());
    }
  
    // Append gender if defined
    if (userParams.gender) {
      params = params.append('gender', userParams.gender);
    }

    // Append order by if defined
    if (userParams.gender) {
      params = params.append('orderBy', userParams.orderBy);
    }
  
    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http)
      .pipe(map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      }))
  }
  

  getMember(username: string) {
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.userName === username);

    if (member) {
      return of(member);
    }
    // Fetch member from the API if not cached
    return this.http.get<Member>(this.baseUrl + 'users/' + username);

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

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/' + 'set-main-photo/ ' + photoId, {});
  }


  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  addLike(username: string) {
    const url = this.baseUrl + 'likes/' + username;
    console.log('URL being sent:', url);
    return this.http.post(url, {});
  }
  

  getLikes(predicate: string, pageNumber, pageSize) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    return getPaginatedResult<Partial<Member[]>>(this.baseUrl + 'likes/' , params, this.http)
  }

}
