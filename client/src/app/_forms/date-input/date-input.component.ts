import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-date-input',
  templateUrl: './date-input.component.html',
  styleUrls: ['./date-input.component.css']
})
export class DateInputComponent implements ControlValueAccessor {
  
  @Input() label: string;
  @Input() maxDate: Date;
  bsConfig: Partial<BsDatepickerConfig>;
  
  value: Date; // Holds the current date value
  
  private onChange = (value: any) => {}; // Callback for when value changes
  private onTouched = () => {}; // Callback for when input is touched
  
  constructor(@Self() public ngControl: NgControl) { 
    this.ngControl.valueAccessor = this;
    this.bsConfig = {
      containerClass: 'theme-dark-blue',
      dateInputFormat: 'DD MMMM YYYY',
      showWeekNumbers: false,
      isAnimated: true,
      adaptivePosition: true,
      customTodayClass: 'custom-today-highlight',
      minMode: 'day',
    };
  }

  // This method is called by Angular forms to write the value to the view
  writeValue(value: any): void {
    if (value) {
      this.value = new Date(value); // Ensure we have a Date object
    }
  }

  // This method registers the callback function to notify the form control when the input value changes
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  // This method registers the callback function to notify when the input is touched
  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  // This method is called when the date changes
  onDateChange(event: any): void {
    this.onChange(event); // Notify the form control
    this.onTouched(); // Notify that the input has been touched
  }
}
