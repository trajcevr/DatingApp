import { Component, EventEmitter, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  @Output() cancel = new EventEmitter<void>(); // Notify parent to close the form
  registerForm: FormGroup;
  maxDate: Date;
  validationErrors: string[] = [];

  constructor(
    private accountService: AccountService,
    private toastr: ToastrService,
    private fb: FormBuilder,
    private router: Router
  ) { }

  ngOnInit() {
    this.initializeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  register() {
    this.accountService.register(this.registerForm.value).subscribe({
      next: (response) => {
        this.router.navigateByUrl('/members');
      },
      error: (error) => {
        console.log(error);
        this.validationErrors = error;
      },
    });
  }

  closeForm() {
    this.cancel.emit(); // Notify parent to close the form
  }

  initializeForm() {
    this.registerForm = this.fb.group(
      {
        gender: ['male', Validators.required], // Default gender is 'male'
        username: ['', Validators.required], // Username is required
        knownAs: ['', Validators.required], // Known as is required
        dateOfBirth: ['', Validators.required], // Date of birth is required
        city: ['', Validators.required], // City is required
        country: ['', Validators.required], // Country is required
        password: ['', [Validators.required, Validators.minLength(6)]], // Password validation
        confirmPassword: ['', Validators.required], // Confirm password validation
      },
      { validators: this.passwordMatchValidator } // Apply the custom validator
    );
  }

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;

    if (password !== confirmPassword) {
      return { passwordMismatch: true };
    }
    return null;
  }
}
