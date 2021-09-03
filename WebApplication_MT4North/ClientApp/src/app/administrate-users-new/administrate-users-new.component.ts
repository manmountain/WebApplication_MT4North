import { Component, OnDestroy, OnInit, ElementRef, ViewChild } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ViewService } from "../_services";
import { AdminService, AccountService, AlertService, ProjectService } from '@app/_services';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { BehaviorSubject, Observable, throwError, Subject, Subscription } from 'rxjs';
import { first, map, tap, catchError } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { Project, UserProject, Theme, Activity, ActivityInfo, ProjectRole, ProjectRights, User } from '@app/_models';
//import { AdminService } from '../_services/admin.service';


@Component({
  selector: 'app-my-pages-administrate-users-new',
  templateUrl: './administrate-users-new.component.html',
  styleUrls: ['./administrate-users-new.component.css']
})

export class AdministrateUsersNew implements OnDestroy {
  isFullscreen: boolean = false;
  isDataLoaded: boolean = false;
  newForm: FormGroup;
  public newpassword = '';

  constructor(
    private viewService: ViewService,
    private adminService: AdminService,
    private accountService: AccountService,
    private route: ActivatedRoute,
    private alertService: AlertService,
    private router: Router
  ) {

  }



  ngOnInit() {
    this.newForm = new FormGroup({
      email: new FormControl()
    });
    this.isDataLoaded = true;
  }

  ngOnDestroy() {
    this.newpassword = '';
  }

  registerUser() {
    let email = this.newForm.get('email').value;
    let password = this.adminService.generatePassword();
    this.adminService.registerUser(email, password)
      .pipe(first())
      .subscribe(
        data => {
          this.newpassword = password;
          this.alertService.success('Användaren har skapats');
        },
        error => {
          console.log('error', error);
          const err = error.error.message || error.statusText;
          this.alertService.error(err);
        });
  }

  back() {
    this.router.navigate(['/my-pages/administrate-users']);
  }
}
