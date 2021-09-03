import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { map, tap, catchError } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { Project, UserProject, Theme, Activity, ActivityInfo, ProjectRole, ProjectRights, User} from '@app/_models';


@Injectable({
  providedIn: 'root'
})
export class AdminService {
  isFullscreen: boolean = false;

  constructor(private http: HttpClient) {
  }

  getBaseActivityInfo(id: number) {
    return this.http.get<ActivityInfo>(`${environment.apiUrl}/BaseActivityInfos/${id}`).pipe(tap(
      activityInfo => {
        return activityInfo;
      },
      error => {
        return error;
      }
    ));
  }

  getBaseActivityInfos() {
    return this.http.get<ActivityInfo[]>(`${environment.apiUrl}/BaseActivityInfos`).pipe(tap(
      activityInfo => {
        return activityInfo;
      },
      error => {
        return error;
      }
    ));
  }

  editBaseActivityInfo(baseactivityinfo: ActivityInfo, id: number) {
    return this.http.put<ActivityInfo>(`${environment.apiUrl}/BaseActivityInfos/${id}`, baseactivityinfo).pipe(tap(
      activityInfo => {
        return activityInfo;
      },
      error => {
        return error;
      }
    ));
  }

  createBaseActivityInfo(baseactivityinfo: ActivityInfo) {
    return this.http.post<ActivityInfo>(`${environment.apiUrl}/BaseActivityInfos`, baseactivityinfo).pipe(tap(
      activityInfo => {
        return activityInfo;
      },
      error => {
        return error;
      }
    ));
  }

  deleteBaseActivityInfo(id: number) {
    return this.http.delete<ActivityInfo>(`${environment.apiUrl}/BaseActivityInfos/${id}`).pipe(tap(
      activityInfo => {
        return activityInfo;
      },
      error => {
        return error;
      }
    ));
  }

  getThemes() {
    return this.http.get<Theme[]>(`${environment.apiUrl}/Themes`).pipe(tap(
      themes => {
        return themes;
      },
      error => {
        return error;
      }
    ));
  }

  getTheme(id: number) {
    return this.http.get<Theme>(`${environment.apiUrl}/Themes/${id}`).pipe(tap(
      theme => {
        return theme;
      },
      error => {
        return error;
      }
    ));
  }

  editTheme(theme: Theme, id: number) {
    return this.http.put<Theme>(`${environment.apiUrl}/Themes/${id}`, theme).pipe(tap(
      theme => {
        return theme;
      },
      error => {
        return error;
      }
    ));
  }

  createTheme(theme: Theme) {
    return this.http.post<Theme>(`${environment.apiUrl}/Themes`, theme).pipe(tap(
      theme => {
        return theme;
      },
      error => {
        return error;
      }
    ));
  }

  deleteTheme(id: number) {
    return this.http.delete<Theme>(`${environment.apiUrl}/Themes/${id}`).pipe(tap(
      theme => {
        return theme;
      },
      error => {
        return error;
      }
    ));
  }

  getRoles() {
    return this.http.get<string[]>(`${environment.apiUrl}/Account/roles`).pipe(tap(
      roles => {
        console.log(roles);
        return roles;
      },
      error => {
        console.log('error', error);
        return error;
      }
    ));
  }

  getUsers() {
    return this.http.get<User[]>(`${environment.apiUrl}/Account/Users`).pipe(tap(
      themes => {
        return themes;
      },
      error => {
        return error;
      }
    ));
  }

  getUser(id: string) {
    return this.http.get<User>(`${environment.apiUrl}/Account/User/${id}`).pipe(tap(
      user => {
        return user;
      },
      error => {
        return error;
      }
    ));
  }

  editUser(user: User, id: string) {
    return this.http.put<User>(`${environment.apiUrl}/Account/User/${id}`, user).pipe(tap(
      theme => {
        return theme;
      },
      error => {
        return error;
      }
    ));
  }

  makeUserAdmin(id: string) {
    return this.http.put<User>(`${environment.apiUrl}/Account/User/roles/admin/${id}`,"").pipe(tap(
      user => {
        return user;
      },
      error => {
        return error;
      }
    ));
  }

  makeUserBasic(id: string) {
    return this.http.put<User>(`${environment.apiUrl}/Account/User/roles/basic/${id}`,"").pipe(tap(
      user => {
        return user;
      },
      error => {
        return error;
      }
    ));
  }

  deleteUser(id: string) {
    return this.http.delete<any>(`${environment.apiUrl}/Account/User/${id}`).pipe(tap(
      result => {
        return result;
      },
      error => {
        return error;
      }
    ));
  }

  registerUser(email: string, password: string) {
    let body = { email: email, password: password };
    return this.http.post<any>(`${environment.apiUrl}/Account/register`, body).pipe(tap(
      StatusResult => {
        return StatusResult;
      },
      error => {
        return error;
      }
    ));

  }

  setPasswordById(id: string, newpassword: string) {
    let body = { new_password: newpassword, current_password: "magicunicorns" };
    console.log('request', body);
    return this.http.put<any>(`${environment.apiUrl}/Account/User/password/${id}`, body).pipe(tap(
      StatusResult => {
        return StatusResult;
      },
      error => {
        return error;
      }
    ));
  }

  /* password generating */
  generatePassword() {
    let password = '';
    let specials = '!@#$&()_-+'; //!@#$%^&*()_+{}:"<>?\|[];\',./`~
    let lowercase = 'abcdefghijklmnopqrstuvwxyz';
    let uppercase = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';
    let numbers = '0123456789';
    let all = specials + lowercase + uppercase + numbers;

    password += this.pick(password, specials, 1, 3);
    password += this.pick(password, lowercase, 1, 3);
    password += this.pick(password, uppercase, 1, 3);
    password += this.pick(password, numbers, 1, 3);
    password += this.pick(password, all, 7, 7);

    return this.shuffle(password);
  }

  pick(exclusions: string, _string: string, min: number, max: number) {
    var n, chars = '';
    if (max === undefined) {
      n = min;
    } else {
      n = min + Math.floor(Math.random() * (max - min + 1));
    }
    var i = 0;
    while (i < n) {
      const character = _string.charAt(Math.floor(Math.random() * _string.length));
      if (exclusions.indexOf(character) < 0 && chars.indexOf(character) < 0) {
        chars += character;
        i++;
      }
    }
    return chars;
  }

  shuffle(_string: string) {
    var array = _string.split('');
    var tmp, current, top = array.length;

    if (top) while (--top) {
      current = Math.floor(Math.random() * (top + 1));
      tmp = array[current];
      array[current] = array[top];
      array[top] = tmp;
    }
    return array.join('');
  }

}
