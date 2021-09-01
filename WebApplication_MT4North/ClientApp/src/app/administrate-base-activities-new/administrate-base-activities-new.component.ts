import { Component, OnDestroy, OnInit, ElementRef, ViewChild } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ViewService } from "../_services";
import { AdminService, AccountService, AlertService, ProjectService } from '@app/_services';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { BehaviorSubject, Observable, throwError, Subject, Subscription } from 'rxjs';
import { first, map, tap, catchError } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { Project, UserProject, Theme, Activity, ActivityInfo, ProjectRole, ProjectRights } from '@app/_models';


@Component({
  selector: 'app-my-pages-administrate-base-activities-new',
  templateUrl: './administrate-base-activities-new.component.html',
  styleUrls: ['./administrate-base-activities-new.component.css']
})

export class AdministrateBaseActivitiesNew implements OnDestroy {
  isFullscreen: boolean = false;
  isDataLoaded: boolean = false;
  newForm: FormGroup;
  private sub: any;
  public baseactivity: ActivityInfo;
  public themes: Theme[];
  public phases: any[] = [{ id: 0, name: "Konceptualisering" }, { id: 1, name: "Konceptvalidering" }, { id: 2, name: "Produktutveckling" }, { id: 3, name: "Produktlansering" }];

  public formId: string = "0";
  public formName: string = "";
  public formDescription: string = "";

  constructor(
    private viewService: ViewService,
    private adminService: AdminService,
    private alertService: AlertService,
    private router: Router
  ) {
  }

  ngOnInit() {
    this.adminService.getThemes()
      .pipe(first())
      .subscribe(
        data => {
          this.themes = data;
          this.newForm = new FormGroup({
            id: new FormControl({ disabled: true }),
            name: new FormControl(),
            description: new FormControl(),
            theme: new FormControl(this.themes[0]),
            phase: new FormControl(this.phases[0])
          });
          this.isDataLoaded = true;
        },
        error => {
          const err = error.error.message || error.statusText;
          this.alertService.error(err);
        });
  }

  ngOnDestroy() {
  }

  newActivity() {
    this.baseactivity = new ActivityInfo();
    this.baseactivity.baseactivityid = 0;
    this.baseactivity.name = this.newForm.get('name').value;
    this.baseactivity.description = this.newForm.get('description').value;
    this.baseactivity.theme = null;
    this.baseactivity.themeid = this.newForm.get('theme').value.themeid;
    this.baseactivity.phase = this.newForm.get('phase').value.id;
    this.adminService.createBaseActivityInfo(this.baseactivity)
      .pipe(first())
      .subscribe(
        data => {
          this.baseactivity = data;
          this.alertService.success('Basaktiviteten har skapats', { keepAfterRouteChange: true });
          this.router.navigate(['/my-pages/administrate-base-activities']);
        },
        error => {
          const err = error.error.message || error.statusText;
          this.alertService.error(err);
        });
  }

  back() {
    this.router.navigate(['/my-pages/administrate-base-activities']);
  }

}
