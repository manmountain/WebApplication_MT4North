import { Component, ElementRef, ViewChild } from '@angular/core';
import { ViewService } from "../_services";
import { AlertService, AccountService, ProjectService } from '@app/_services';
import { User, Project } from '../_models';
import { first } from 'rxjs/operators';

@Component({
  selector: 'app-my-pages',
  templateUrl: './my-pages.component.html',
  styleUrls: ['./my-pages.component.css']
})

export class MyPagesComponent {
  isFirstStepModal = true;
  emailList = [];
  currentUser: User;
  projects: Project[];
  error = '';

  constructor(
    private viewService: ViewService,
    private alertService: AlertService,
    private accountService: AccountService,
    private projectService: ProjectService
  ) {
    this.accountService.currentUser.subscribe(x => { this.currentUser = x; console.log('subscribe user: ', this.currentUser); }, e => console.log(JSON.stringify(e)));
    this.projectService.getProjects()
      .pipe(first())
      .subscribe(
        data => { },

        error => {
          this.error = error;
          this.alertService.error(error);
        });
    this.projectService.projects.subscribe(x => this.projects = x);
    console.log("projects in my-pages: ", this.projects[0].name);

 }

  isFullscreen() {
    return this.viewService.isFullscreen;
  }

  changeToInviteMembers() {
    this.isFirstStepModal = false;
  }

  resetModal() {
    this.isFirstStepModal = true;
    this.emailList = [];
  }

  addMember(emailAdress: string) {
    console.log(emailAdress);
    this.emailList.push(emailAdress);
  }

  removeMember(emailAdress: string) {
    this.emailList.splice(0,1);
  }
}
