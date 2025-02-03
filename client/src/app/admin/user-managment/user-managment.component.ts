import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-managment',
  templateUrl: './user-managment.component.html',
  styleUrls: ['./user-managment.component.css']
})
export class UserManagmentComponent implements OnInit {
  users: User[] = [];
  bsModalRef: BsModalRef;


  constructor(private adminService: AdminService, private modalService: BsModalService) { }

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles(): void {
    this.adminService.getUsersWithRoles().subscribe(users => {
      console.log('Users retrieved (full object):', users);
      console.log('First user properties:', Object.keys(users[0]));
      this.users = users;
    });
  }

  openRolesModal(user: User): void {
    console.log('Full user object:', user);
    console.log('User object keys:', Object.keys(user));
    console.log('Attempting to access userName:', user.userName);

    const config = {
      initialState: {
        user: user,
        roles: this.getRolesArray(user)
      }
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);

    this.bsModalRef.content.updateSelectedRoles.subscribe(roles => {
      const rolesToUpdate = roles.filter(r => r.checked).map(r => r.name);

      if (rolesToUpdate.length > 0) {
        this.adminService.updateUserRoles(user.userName, rolesToUpdate).subscribe({
          next: () => user.roles = rolesToUpdate,
          error: (err) => console.error('Role update error:', err)
        });
      }
    });
  }


  private getRolesArray(user: User): any[] {
    const userRolesSet = new Set(user.roles); // Use Set for quicker lookup
    const availableRoles = [
      { name: 'Admin', value: 'Admin' },
      { name: 'Moderator', value: 'Moderator' },
      { name: 'Member', value: 'Member' }
    ];

    return availableRoles.map(role => ({
      ...role,
      checked: userRolesSet.has(role.name)
    }));
  }
}
