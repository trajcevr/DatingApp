import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  @Output() cancel = new EventEmitter<void>(); // Event to notify parent component

  model: any = {};

  constructor(private accountService: AccountService) { }

  register() {
    this.accountService.register(this.model).subscribe(response => {
      console.log(response);
      this.closeForm();
    }, error => {
      console.log(error);
    })
  }

  closeForm() {
    this.cancel.emit(); // Notify parent to close the form
  }
}
