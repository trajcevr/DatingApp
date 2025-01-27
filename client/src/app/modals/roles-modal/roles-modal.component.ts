import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';
import { EventEmitter } from '@angular/core';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent implements OnInit {
  @Input() updateSelectedRoles = new EventEmitter<any[]>();
  user: User;
  roles: any[];

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
    console.log('User in modal:', this.user);
    console.log('Roles in modal:', this.roles);
  }

  updateRoles() {
    if (this.roles) {
      console.log('Emitting roles:', this.roles);
      this.updateSelectedRoles.emit(this.roles);
      this.bsModalRef.hide();
    }
  }
}