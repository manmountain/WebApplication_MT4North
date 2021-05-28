import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '@environments/environment';
import { AccountService } from '@app/_services';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private accountService: AccountService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // add auth header with jwt if user is logged in and request is to the api url
    console.log('starts adding header');
    const currentUser = this.accountService.currentUserValue;
    const isLoggedIn = currentUser && currentUser.accessToken;
    const isApiUrl = request.url.startsWith(environment.apiUrl);
    console.log('currentUser: ', this.accountService.currentUserValue);
    //console.log('currentUser.token: ', currentUser.token);

    console.log('isLogged in? ', isLoggedIn);
    console.log('isApiUrl? ', isApiUrl);

    if (isLoggedIn && isApiUrl) {
      console.log('isLogged in and is ApiUrl');
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${currentUser.accessToken}`
        }
      });
    }

    return next.handle(request);
  }
}
