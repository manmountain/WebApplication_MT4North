import { Component, OnDestroy, OnInit, ElementRef, ViewChild } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ViewService } from "../_services";
import { AdminService, AccountService, AlertService, ProjectService } from '@app/_services';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { BehaviorSubject, Observable, throwError, Subject, Subscription } from 'rxjs';
import { first, map, tap, catchError } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { Project, UserProject, Theme, Activity, ActivityInfo, ProjectRole, ProjectRights } from '@app/_models';


@Component({
  selector: 'app-my-pages-administrate-base-activities',
  templateUrl: './administrate-base-activities.component.html',
  styleUrls: ['./administrate-base-activities.component.css']
})

export class AdministrateBaseActivities implements OnDestroy {
  isFullscreen: boolean = false;
  public baseactivityinfos: ActivityInfo[];

  constructor(
    private viewService: ViewService,
    private adminService: AdminService,
    private alertService: AlertService,
    private router: Router
  ) {
    console.log('constructor AdministrateBaseActivities');
  }

  ngOnInit() {
    this.adminService.getBaseActivityInfos()
      .pipe(first())
      .subscribe(
        data => {
          this.baseactivityinfos = data;
        },
        error => {
          const err = error.error.message || error.statusText;
          this.alertService.error(err);
        });
  }

  removeActivity(id: number) {
    this.adminService.deleteBaseActivityInfo(id)
      .pipe(first())
      .subscribe(
        data => {
          this.baseactivityinfos.forEach((baseactivityinfo, index) => {
            if (baseactivityinfo.baseactivityid === data.baseactivityid) this.baseactivityinfos.splice(index, 1);
          });
          this.alertService.success('Basaktiviteten har tagits bort', { keepAfterRouteChange: false });
        },
        error => {
          const err = error.error.message || error.statusText;
          this.alertService.error(err);
        });
  }

  ngOnDestroy() {
    this.baseactivityinfos = null;
  }

}
