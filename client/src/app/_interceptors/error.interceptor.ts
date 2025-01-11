import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';
import { Router, NavigationExtras } from '@angular/router';  // Use Router here

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
        // Handling different error statuses
        if (error) {
          switch (error.status) {
            case 400:  // Bad Request
              if (error.error.errors) {
                const modalStateErrors = [];
                for (const key in error.error.errors) {
                  if (error.error.errors[key]) {
                    modalStateErrors.push(error.error.errors[key]);
                  }
                }
                throw modalStateErrors.flat();
              } else {
                this.toastr.error(error.statusText, error.status);
              }
              break;
            case 401:  // Unauthorized
              this.toastr.error('You are not authorized to perform this action.', error.status);
              break;
            case 404:  // Not Found
              this.router.navigateByUrl('/not-found');
              break;
            case 500:  // Internal Server Error
              // Pass error details to a specific error page
              const navigationExtras: NavigationExtras = {
                state: { error: error.error }
              };
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              this.toastr.error('An unexpected error occurred.');
              break;
          }
        }
        return throwError(() => error);
      })
    );
  }
}
