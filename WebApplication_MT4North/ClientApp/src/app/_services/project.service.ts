import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from '@environments/environment';
import { Project, UserProject } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class ProjectService {
  private projectSubjects: BehaviorSubject<Project[]>;
  public projects: Observable<Project[]>;

  private selectedProjectSubject: BehaviorSubject<UserProject[]>;
  public selectedProject: Observable<UserProject[]>;

  constructor(private http: HttpClient) {
    this.projectSubjects = new BehaviorSubject<Project[]>(JSON.parse(localStorage.getItem('currentProjects')));
    this.projects = this.projectSubjects.asObservable();

    this.selectedProjectSubject = new BehaviorSubject<UserProject[]>(JSON.parse(localStorage.getItem('selectedProject')));
    this.selectedProject = this.selectedProjectSubject.asObservable();
  }

  public get currentProjectsValue(): Project[] {
    return this.projectSubjects.value;
  }

  public get selectedProjectValue(): UserProject[] {
    return this.selectedProjectSubject.value;
  }

  createProject(title: string, description: string) {
    return this.http.post<any>(`${environment.apiUrl}/Projects`, { title, description }).pipe(map(project => {
      // store user details and jwt token in local storage to keep user logged in between page refreshes

      //localStorage.setItem('currentProjects', JSON.stringify(project));
      //this.projectSubjects.next(project);
      return project;
    }));
  }



  getProjects() {
    console.log('getting projects ');

    return this.http.get<Project[]>(`${environment.apiUrl}/Projects`).pipe(map(projects => {
      // store user details and jwt token in local storage to keep user logged in between page refreshes
      console.log('projects value: ', this.currentProjectsValue);

      localStorage.setItem('currentProjects', JSON.stringify(projects));
      this.projectSubjects.next(projects);
      return projects;
    }));;
  }

  selectProject(projectId: string) {
    console.log('getting user projects ');

    return this.http.get<UserProject[]>(`${environment.apiUrl}/UserProjects/Project/${projectId}`).pipe(map(selectedProject => {
      // store user details and jwt token in local storage to keep user logged in between page refreshes
      console.log('selected project value: ', this.selectedProjectValue);

      localStorage.setItem('selectedProject', JSON.stringify(selectedProject));
      this.selectedProjectSubject.next(selectedProject);
      return selectedProject;
    }));;
  }

  update(params) {
    //console.log(params);
    return this.http.put(`${environment.apiUrl}/Project`, params);
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
