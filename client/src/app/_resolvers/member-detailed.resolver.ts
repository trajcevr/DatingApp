import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from "@angular/router";
import { Member } from "../_models/member";
import { MembersService } from "../_services/members.service";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
  })
  export class MemberDetailedResolver implements Resolve<Member> {

    constructor(private membersService: MembersService) {}
  
    resolve(route: ActivatedRouteSnapshot): Observable<Member> {
      const username = route.paramMap.get('username');
      if (username) {
        return this.membersService.getMember(username); 
      } 
      else {
        return null;
      }
    }
  }
  