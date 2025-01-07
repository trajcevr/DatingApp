import { HttpClient } from '@angular/common/http';
import { Component, ElementRef, ViewChild } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent {
  showRegisterForm = false; // Tracks whether the register form is visible
  constructor() {

  }

  @ViewChild('registerFormContainer') registerFormContainer!: ElementRef;

  ngOnInit(): void {

  }
  toggleRegisterForm() {
    this.showRegisterForm = !this.showRegisterForm;
    if (this.showRegisterForm) {
      setTimeout(() => {
        this.scrollToRegisterForm();
      }, 0);
    }
  }

  scrollToRegisterForm() {
    this.registerFormContainer?.nativeElement.scrollIntoView({
      behavior: 'smooth',
      block: 'start',
    });
  }


}
