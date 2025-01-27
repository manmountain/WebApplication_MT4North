import { Component, OnDestroy, OnInit, ElementRef, ViewChild } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ViewService } from "../_services";
import { AdminService, AccountService, AlertService, ProjectService } from '@app/_services';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { BehaviorSubject, Observable, throwError, Subject, Subscription } from 'rxjs';
import { first, map, tap, catchError } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { Project, UserProject, Theme, Activity, ActivityInfo, ProjectRole, ProjectRights, User } from '@app/_models';
//import { AdminService } from '../_services/admin.service';


@Component({
  selector: 'app-my-pages-administrate-users',
  templateUrl: './administrate-users.component.html',
  styleUrls: ['./administrate-users.component.css']
})

export class AdministrateUsers implements OnDestroy {
  isFullscreen: boolean = false;
  public users: User[];
  public currentUser: User;

  constructor(
    private viewService: ViewService,
    private adminService: AdminService,
    private accountService: AccountService,
    private router: Router
  ) {

  }

  ngOnInit() {
    this.currentUser = this.accountService.currentUserValue;
    this.adminService.getUsers()
      .pipe(first())
      .subscribe(
        data => {
          this.users = data;
        },
        error => {
          console.log('AdministrateUsers ngInit error', error);
          // alert service ...
        });
  }

  ngOnDestroy() {
    this.users = null;
  }

  removeUser(id: string) {
    this.adminService.deleteUser(id)
      .pipe(first())
      .subscribe(
        data => {
          const id = data.message.split(" ")[1];
          console.log('remove user', data);
          this.users.forEach((user, index) => {
            if (user.id === id) this.users.splice(index, 1);
          });
          // alert..
        },
        error => {
          console.log('AdministrateUsers ngInit error', error);
          //alert..
        });
  }


}
