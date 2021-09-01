import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { map, tap, catchError } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { Project, UserProject, Theme, Activity, ActivityInfo, ProjectRole, ProjectRights, User } from '@app/_models';


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

}
