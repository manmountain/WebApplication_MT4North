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

  private userProjectsSubject: BehaviorSubject<UserProject[]>;
  public userProjects: Observable<UserProject[]>;

  private invitationsSubject: BehaviorSubject<UserProject[]>;
  public invitations: Observable<UserProject[]>;

  constructor(private http: HttpClient) {
    this.projectSubjects = new BehaviorSubject<Project[]>(JSON.parse(localStorage.getItem('currentProjects')));
    this.projects = this.projectSubjects.asObservable();

    this.userProjectsSubject = new BehaviorSubject<UserProject[]>(JSON.parse(localStorage.getItem('userProjects')));
    this.userProjects = this.userProjectsSubject.asObservable();

    this.invitationsSubject = new BehaviorSubject<UserProject[]>(JSON.parse(localStorage.getItem('invitations')));
    this.invitations = this.invitationsSubject.asObservable();
  }

  public get currentProjectsValue(): Project[] {
    return this.projectSubjects.value;
  }

  public get userProjectsValue(): UserProject[] {
    return this.userProjectsSubject.value;
  }

  public get invitationsValue(): UserProject[] {
    return this.invitationsSubject.value;
  }

  createProject(name: string, description: string) {
    return this.http.post<any>(`${environment.apiUrl}/Projects`, { name, description }).pipe(map(project => {
      // store user details and jwt token in local storage to keep user logged in between page refreshes

      //localStorage.setItem('currentProjects', JSON.stringify(project));
      //this.projectSubjects.next(project);
      this.currentProjectsValue.push(project);
      this.projectSubjects.next(this.currentProjectsValue);

      return project;
    }));
  }



  getProjects() {
    console.log('getting projects ');

    return this.http.get<Project[]>(`${environment.apiUrl}/Projects`).pipe(map(projects => {
      // store user details and jwt token in local storage to keep user logged in between page refreshes
      console.log('projects value in service: ', this.currentProjectsValue);

      localStorage.setItem('currentProjects', JSON.stringify(projects));
      this.projectSubjects.next(projects);
      return projects;
    }));;
  }

  getUserProjects(projectId: string) {
    console.log('getting user projects ');

    return this.http.get<UserProject[]>(`${environment.apiUrl}/UserProjects/Project/${projectId}`).pipe(map(userProjects => {
      // store user details and jwt token in local storage to keep user logged in between page refreshes
      console.log('selected project value in service: ', this.userProjectsValue);

      localStorage.setItem('userProjects', JSON.stringify(userProjects));
      this.userProjectsSubject.next(userProjects);
      return userProjects;
    }));;
  }

  update(params) {
    console.log(params);
    return this.http.put(`${environment.apiUrl}/Projects/${this.userProjectsValue[0].projectId}`, params).pipe(map(x => {
      {

        // update local storage
        const project = { ...this.userProjects, ...params };
        ////console.log('PROJECT: ', project);
        //  localStorage.setitem('selectedProject', JSON.stringify(project));
        this.userProjectsValue[0].project = project;
        let projectToUpdate = this.currentProjectsValue.find(y => y.projectId == this.userProjectsValue[0].projectId);
        let index = this.currentProjectsValue.indexOf(projectToUpdate);

        this.currentProjectsValue[index] = project;

          // publish updated user to subscribers
        this.userProjectsSubject.next(this.userProjectsValue);
        this.projectSubjects.next(this.currentProjectsValue);

        }
        return x;
      }));
  }

  getInvites() {
    return this.http.get<any>(`${environment.apiUrl}/UserProjects/Invites`).pipe(map(invitations => {
      // store user details and jwt token in local storage to keep user logged in between page refreshes

      //localStorage.setItem('currentProjects', JSON.stringify(project));
      //this.projectSubjects.next(project);

      this.invitationsSubject.next(invitations);

      return invitations;
    }));
  }

  inviteMember(projectId: number, email: string, role: string, permissions: string) {
    return this.http.post<any>(`${environment.apiUrl}/UserProjects/${email}/${projectId}/${role}/${permissions}`, '').pipe(map(userProject => {
      // store user details and jwt token in local storage to keep user logged in between page refreshes

      //localStorage.setItem('currentProjects', JSON.stringify(project));
      //this.projectSubjects.next(project);
      console.log('invited member');
      this.userProjectsValue.push(userProject);
      this.userProjectsSubject.next(this.userProjectsValue);

      return userProject;
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
