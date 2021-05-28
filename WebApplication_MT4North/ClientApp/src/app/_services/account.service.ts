import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from '@environments/environment';
import { User } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class AccountService {
    private currentUserSubject: BehaviorSubject<User>;
    public currentUser: Observable<User>;

  constructor(private http: HttpClient) {
    this.currentUserSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('currentUser')));
    this.currentUser = this.currentUserSubject.asObservable();
    console.log('created currentUser: ', this.currentUserValue);

  }

    public get currentUserValue(): User {
        return this.currentUserSubject.value;
    }
  
    login(email: string, password: string) {
          return this.http.post<any>(`${environment.apiUrl}/Account/login`, { email, password })
              .pipe(map(user => {
                  // store user details and jwt token in local storage to keep user logged in between page refreshes
                
                  localStorage.setItem('currentUser', JSON.stringify(user));
                this.currentUserSubject.next(user);
                  return user;
              }));
      }

    logout() {
        // remove user from local storage to log user out
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
    }

    register(user: User) {
      return this.http.post(`${environment.apiUrl}/Account/register`, user);
    }

    getAll() {
      return this.http.get<User[]>(`${environment.apiUrl}/users`);
    } 

  getCurrentUser() {
    console.log('testar');
      return this.http.get<User>(`${environment.apiUrl}/Account/user`);
  }

    getById(id: string) {
      return this.http.get<User>(`${environment.apiUrl}/users/${id}`);
  }

  update(params) {
    //console.log(params);
      return this.http.put(`${environment.apiUrl}/Account/user`, params)
        .pipe(map(x => {
           {
            // update local storage
            const user = { ...this.currentUserValue, ...params };
            localStorage.setItem('user', JSON.stringify(user));

            // publish updated user to subscribers
            this.currentUserSubject.next(user);
          }
          return x;
        }));
    }

    //update(id, params) {
    //  return this.http.put(`${environment.apiUrl}/users/${id}`, params)
    //    .pipe(map(x => {
    //      // update stored user if the logged in user updated their own record
    //      if (id == this.userValue.id) {
    //        // update local storage
    //        const user = { ...this.userValue, ...params };
    //        localStorage.setItem('user', JSON.stringify(user));

    //        // publish updated user to subscribers
    //        this.userSubject.next(user);
    //      }
    //      return x;
    //    }));
    //}

    //delete(id: string) {
    //  return this.http.delete(`${environment.apiUrl}/users/${id}`)
    //    .pipe(map(x => {
    //      // auto logout if the logged in user deleted their own record
    //      if (id == this.userValue.id) {
    //        this.logout();
    //      }
    //      return x;
    //    }));
    //}
}
