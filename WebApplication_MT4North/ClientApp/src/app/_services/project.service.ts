import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
//import { throwError } from 'rxjs'; 
import { map, tap, catchError } from 'rxjs/operators';

import { environment } from '@environments/environment';
import { Project, UserProject, Theme, Activity, ActivityInfo, ProjectRole, ProjectRights } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class ProjectService {
  private projectSubjects: BehaviorSubject<Project[]>;
  public projects: Observable<Project[]>;

  private userProjectsSubject: BehaviorSubject<UserProject[]>;
  public userProjects: Observable<UserProject[]>;

  private invitationsSubject: BehaviorSubject<UserProject[]>;
  public invitations: Observable<UserProject[]>;

  private themesSubject: BehaviorSubject<Theme[]>;
  public themes: Observable<Theme[]>;

  private selectedProjectSubject: BehaviorSubject<Project>;
  public selectedProject: Observable<Project>;

  private activitiesSubject: BehaviorSubject<Activity[]>;
  public activities: Observable<Activity[]>;

  constructor(private http: HttpClient) {
    //this.projectSubjects = new BehaviorSubject<Project[]>(new Array<Project>());
    //this.projects = this.projectSubjects.asObservable();

    //this.userProjectsSubject = new BehaviorSubject<UserProject[]>(new Array<UserProject>());
    //this.userProjects = this.userProjectsSubject.asObservable();

    //this.selectedProjectSubject = new BehaviorSubject<Project>(new Project());
    //this.selectedProject = this.selectedProjectSubject.asObservable();

    //this.invitationsSubject = new BehaviorSubject<UserProject[]>(new Array<UserProject>());
    //this.invitations = this.invitationsSubject.asObservable();

    this.themesSubject = new BehaviorSubject<Theme[]>(new Array<Theme>());
    this.themes = this.themesSubject.asObservable();

    this.activitiesSubject = new BehaviorSubject<Activity[]>(new Array<Activity>());
    this.activities = this.activitiesSubject.asObservable();

    this.projectSubjects = new BehaviorSubject<Project[]>(JSON.parse(localStorage.getItem('currentProjects')));
    this.projects = this.projectSubjects.asObservable();

    this.userProjectsSubject = new BehaviorSubject<UserProject[]>(JSON.parse(localStorage.getItem('userProjects')));
    this.userProjects = this.userProjectsSubject.asObservable();

    this.selectedProjectSubject = new BehaviorSubject<Project>(JSON.parse(localStorage.getItem('selectedProject')));
    this.selectedProject = this.selectedProjectSubject.asObservable();

    this.invitationsSubject = new BehaviorSubject<UserProject[]>(JSON.parse(localStorage.getItem('invitations')));
    this.invitations = this.invitationsSubject.asObservable();

    //this.themesSubject = new BehaviorSubject<Theme[]>(JSON.parse(localStorage.getItem('themes')));
    //this.themes = this.themesSubject.asObservable();

    //this.activitiesSubject = new BehaviorSubject<Activity[]>(JSON.parse(localStorage.getItem('activities')));
    //this.activities = this.activitiesSubject.asObservable();
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

  public get selectedProjectValue(): Project {
    return this.selectedProjectSubject.value;
  }

  public get themesValue(): Theme[] {
    return this.themesSubject.value;
  }

  public get activitiesValue(): Activity[] {
    return this.activitiesSubject.value;
  }

  createProject(name: string, description: string) {
    return this.http.post<any>(`${environment.apiUrl}/Projects`, { name, description }).pipe(map(project => {
      // store user details and jwt token in local storage to keep user logged in between page refreshes

      //this.projectSubjects.next(project);
      this.currentProjectsValue.push(project);
      const currentProjects = { ...this.currentProjectsValue, ...project };

      localStorage.setItem('currentProjects', JSON.stringify(currentProjects));
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

  selectProject(projectId: string) {
    return this.http.get<Project>(`${environment.apiUrl}/Projects/${projectId}`).pipe(map(project => {

      localStorage.setItem('selectedProject', JSON.stringify(project));

      this.selectedProjectSubject.next(project);

      return project;
    }));
  }

  getUserProjects(projectId: string) {
    console.log('getting user projects ');

    return this.http.get<UserProject[]>(`${environment.apiUrl}/UserProjects/Project/${projectId}`).pipe(map(userProjects => {
      // store user details and jwt token in local storage to keep user logged in between page refreshes
      //console.log('selected project value in service: ', this.userProjectsValue);
      const getCircularReplacer = () => {
        const seen = new WeakSet();
        return (key, value) => {
          if (typeof value === "object" && value !== null) {
            if (seen.has(value)) {
              return;
            }
            seen.add(value);
          }
          return value;
        };
      };
      localStorage.setItem('userProjects', JSON.stringify(userProjects, getCircularReplacer()));

      this.userProjectsSubject.next(userProjects);

      return userProjects;
    }));;
  }

  updateProject(params) {
    console.log(params);
    return this.http.put(`${environment.apiUrl}/Projects/${this.userProjectsValue[0].projectid}`, params).pipe(map(x => {
      {

        // update local storage
        const project = { ...this.userProjects, ...params };
        ////console.log('PROJECT: ', project);
          //localStorage.setitem('selectedProject', JSON.stringify(project));
        for (var userProject of this.userProjectsValue) {
          userProject.project = project;
        }
        let projectToUpdate = this.currentProjectsValue.find(y => y.projectid == this.userProjectsValue[0].projectid);
        let index = this.currentProjectsValue.indexOf(projectToUpdate);

        this.currentProjectsValue[index] = project;
        console.log('PROJ VALUES: ', this.userProjectsValue);

        const getCircularReplacer = () => {
          const seen = new WeakSet();
          return (key, value) => {
            if (typeof value === "object" && value !== null) {
              if (seen.has(value)) {
                return;
              }
              seen.add(value);
            }
            return value;
          };
        };

        localStorage.setItem('selectedProject', JSON.stringify(project, getCircularReplacer()));

          // publish updated user to subscribers
        this.userProjectsSubject.next(this.userProjectsValue);
        this.projectSubjects.next(this.currentProjectsValue);
        this.selectedProjectSubject.next(project);

        }
        return x;
      }));
  }

  leaveProject(userProjectId: string): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/UserProjects/${userProjectId}`, { observe: 'response' })
      .pipe(
        tap(
          userProject => {
            // remove from localstorage
            let userProjectToDelete = this.userProjectsValue.find(y => y.userprojectid == userProjectId);
            let index = this.userProjectsValue.indexOf(userProjectToDelete);
            if (index > -1) {
              this.userProjectsValue.splice(index, 1);
            }
            localStorage.setItem('userProjects', JSON.stringify(this.userProjectsValue));
            // publish updated userProjects to subscribers
            this.userProjectsSubject.next(this.userProjectsValue);
            return userProject;
          },
          error => {
            // something went wrong
            return throwError(error);
          }
        )
      );
  } 

  updateProjectMember(userProjectId: string, params) {
    console.log(params);
    return this.http.put(`${environment.apiUrl}/UserProjects/${userProjectId}`, params).pipe(map(userProject => {
      {
        let projectToUpdate = this.userProjectsValue.find(y => y.userprojectid == userProjectId);
        let index = this.userProjectsValue.indexOf(projectToUpdate);

        this.userProjectsValue[index] = params;
        const getCircularReplacer = () => {
          const seen = new WeakSet();
          return (key, value) => {
            if (typeof value === "object" && value !== null) {
              if (seen.has(value)) {
                return;
              }
              seen.add(value);
            }
            return value;
          };
        };
        localStorage.setItem('userProjects', JSON.stringify(this.userProjectsValue, getCircularReplacer()));

        console.log('PARMS: ', params);

        // publish updated user to subscribers
        this.userProjectsSubject.next(this.userProjectsValue);
      }
      return userProject;
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

  inviteMember(projectId: string, email: string, role: ProjectRole, permissions: ProjectRights) {
    return this.http.post<any>(`${environment.apiUrl}/UserProjects/${email}/${projectId}/${role}/${permissions}`, '').pipe(map(userProject => {
      // store user details and jwt token in local storage to keep user logged in between page refreshes

      ////localStorage.setItem('currentProjects', JSON.stringify(project));
      ////this.projectSubjects.next(project);
      //console.log('invited member');
      this.userProjectsValue.push(userProject);

      const getCircularReplacer = () => {
        const seen = new WeakSet();
        return (key, value) => {
          if (typeof value === "object" && value !== null) {
            if (seen.has(value)) {
              return;
            }
            seen.add(value);
          }
          return value;
        };
      };
      localStorage.setItem('userProjects', JSON.stringify(this.userProjectsValue, getCircularReplacer()));

      this.userProjectsSubject.next(this.userProjectsValue);

      return userProject;
    }));
  }

  acceptInvite(userProjectId: string) {
    return this.http.post<any>(`${environment.apiUrl}/UserProjects/Invites/Accept/${userProjectId}`, '').pipe(map(userProject => {
      // store user details and jwt token in local storage to keep user logged in between page refreshes

      //localStorage.setItem('currentProjects', JSON.stringify(project));
      //this.projectSubjects.next(project);
      console.log('invite accepted', userProject);
      this.currentProjectsValue.push(userProject.project);
      this.projectSubjects.next(this.currentProjectsValue);

      return userProject;
    }));
  }

  rejectInvite(userProjectId: string) {
    return this.http.post<any>(`${environment.apiUrl}/UserProjects/Invites/Reject/${userProjectId}`, '').pipe(map(userProject => {
      // store user details and jwt token in local storage to keep user logged in between page refreshes

      //localStorage.setItem('currentProjects', JSON.stringify(project));
      //this.projectSubjects.next(project);
      console.log('invited rejected');

      return userProject;
    }));
  }

  getThemes() {
    return this.http.get<Theme[]>(`${environment.apiUrl}/Themes`).pipe(map(themes => {
      const themesConst = { ...this.themesValue, ...themes };

      localStorage.setItem('themes', JSON.stringify(themesConst));
      this.themesSubject.next(themes);

      return themes;
    }));
  }

  getActivities() {
    return this.http.get<Activity[]>(`${environment.apiUrl}/Activities/Project/${this.userProjectsValue[0].projectid}`,).pipe(map(activities => {
      const getCircularReplacer = () => {
        const seen = new WeakSet();
        return (key, value) => {
          if (typeof value === "object" && value !== null) {
            if (seen.has(value)) {
              return;
            }
            seen.add(value);
          }
          return value;
        };
      };
      localStorage.setItem('activities', JSON.stringify(getCircularReplacer()));
      this.activitiesSubject.next(Array.from(activities.values()));

      return Array.from(activities.values());
    }));
  }

  createActivity(params) {
    return this.http.post<any>(`${environment.apiUrl}/Activities`, params).pipe(map(activity => {
      {

        this.activitiesValue.push(activity);
        const activities = { ...this.activitiesValue, ...activity };

        localStorage.setItem('activities', JSON.stringify(activities));
        this.activitiesSubject.next(this.activitiesValue);
      }
      return activity;
    }));
  }

  updateActivity(activityid: number, params) {
    return this.http.put(`${environment.apiUrl}/Activities/${activityid}`, params).pipe(map(activity => {
      {

        let activityToUpdate = this.activitiesValue.find(x => x.activityid == activityid);
        let index = this.activitiesValue.indexOf(activityToUpdate);

        this.activitiesValue[index] = params;

        const getCircularReplacer = () => {
          const seen = new WeakSet();
          return (key, value) => {
            if (typeof value === "object" && value !== null) {
              if (seen.has(value)) {
                return;
              }
              seen.add(value);
            }
            return value;
          };
        };

        // update local storage
        localStorage.setItem('activities', JSON.stringify(this.activities, getCircularReplacer()));

        // publish updated user to subscribers
        this.activitiesSubject.next(this.activitiesValue);

      }
      return activity;
    }));
  }

  addNote(activityid: number, params) {
    console.log('test');
    return this.http.post<any>(`${environment.apiUrl}/Notes/Activity/${activityid}`, params).pipe(map(note => {
      // store user details and jwt token in local storage to keep user logged in between page refreshes

      let activityToUpdate = this.activitiesValue.find(x => x.activityid == activityid);
      let index = this.activitiesValue.indexOf(activityToUpdate);

      this.activitiesValue[index].notes.push(note);

      const getCircularReplacer = () => {
        const seen = new WeakSet();
        return (key, value) => {
          if (typeof value === "object" && value !== null) {
            if (seen.has(value)) {
              return;
            }
            seen.add(value);
          }
          return value;
        };
      };

      // update local storage
      localStorage.setItem('activities', JSON.stringify(this.activities, getCircularReplacer()));

      this.activitiesSubject.next(this.activitiesValue);

      return note;
    }));
  }

  removeNote(activityid: number, noteid:string) {
    return this.http.delete<any>(`${environment.apiUrl}/Notes/${noteid}`).pipe(map(note => {
      // store user details and jwt token in local storage to keep user logged in between page refreshes

      let activityToUpdate = this.activitiesValue.find(x => x.activityid == activityid);
      let index = this.activitiesValue.indexOf(activityToUpdate);

      let noteToRemove = this.activitiesValue[index].notes.find(x => x.noteid == noteid);
      let noteIndex = this.activitiesValue[index].notes.indexOf(noteToRemove);
      this.activitiesValue[index].notes.splice(noteIndex, 1);

      const getCircularReplacer = () => {
        const seen = new WeakSet();
        return (key, value) => {
          if (typeof value === "object" && value !== null) {
            if (seen.has(value)) {
              return;
            }
            seen.add(value);
          }
          return value;
        };
      };

      // update local storage
      localStorage.setItem('activities', JSON.stringify(this.activities, getCircularReplacer()));

      this.activitiesSubject.next(this.activitiesValue);

      return note;
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
