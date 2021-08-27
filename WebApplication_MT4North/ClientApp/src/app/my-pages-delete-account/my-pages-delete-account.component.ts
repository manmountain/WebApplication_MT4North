import { Component, OnDestroy, OnInit, ElementRef, ViewChild } from '@angular/core';
import { ViewService } from "../_services";
import { AccountService, AlertService, ProjectService } from '@app/_services';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { first } from 'rxjs/operators';
import { User, Project, Alert, AlertType } from '../_models';
import { Subscription } from 'rxjs';


@Component({
  selector: 'app-my-pages-delete-account',
  templateUrl: './my-pages-delete-account.component.html',
  styleUrls: ['./my-pages-delete-account.component.css']
})

export class MyPagesDeleteAccountComponent implements OnDestroy {
  projects: Project[];
  currentUser: User;
  accountSubscription: Subscription;
  projectsSubscription: Subscription;
  isAdmin = false;

  constructor(
    private projectService: ProjectService,
    private viewService: ViewService,
    private accountService: AccountService,
    private alertService: AlertService,
  ) {
    this.accountSubscription = this.accountService.currentUser.subscribe(x => { this.currentUser = x; this.isAdmin = this.currentUser.userrole == 'AdminUser'; });

    this.projectsSubscription = this.projectService.projects.subscribe(x => this.projects = x);
    this.projectService.getProjects()
      .pipe(first())
      .subscribe(
        data => {
        },

        error => {
          const err = error.error.message || error.statusText;
          this.alertService.error(err);
        });
  }

  ngOnInit() {

  }

  ngOnDestroy() {
    this.accountSubscription.unsubscribe();
    this.projectsSubscription.unsubscribe();

  }

  isFullscreen() {
    return this.viewService.isFullscreen;
  }

  deleteAccount() {
    this.alertService.clear();

    this.accountService.delete()
      .pipe(first())
      .subscribe(
        data => {
        },
        error => {
          if (error.status == 403) {
            this.alertService.error('Otillåtet. Du måste lämna de projekt du äger eller är deltagare i innan du kan ta bort ditt konto.');
          } else {
            this.alertService.error('Okänt fel. Kontakta support eller försök igen senare. Felkod: ', error.status);
          }
        });
  }
}
