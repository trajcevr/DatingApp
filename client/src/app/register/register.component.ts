import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  @Output() cancel = new EventEmitter<void>(); // Event to notify parent component

  model: any = {};

  constructor(private accountService: AccountService,
    private toastr: ToastrService
  ) { }

  register() {
    this.accountService.register(this.model).subscribe(response => {
      console.log(response);
      this.closeForm();
    }, error => {
      console.log(error);
      this.toastr.error(error.error);
    })
  }

  closeForm() {
    this.cancel.emit(); // Notify parent to close the form
  }
}
