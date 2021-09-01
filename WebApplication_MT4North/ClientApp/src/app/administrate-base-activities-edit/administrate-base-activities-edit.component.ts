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
  selector: 'app-my-pages-administrate-base-activities-edit',
  templateUrl: './administrate-base-activities-edit.component.html',
  styleUrls: ['./administrate-base-activities-edit.component.css']
})

export class AdministrateBaseActivitiesEdit implements OnDestroy {
  isFullscreen: boolean = false;
  isDataLoaded: boolean = false;
  editForm: FormGroup;
  id: number;
  private sub: any;
  public baseactivityinfo: ActivityInfo;
  public themes: Theme[];
  public phases: any[] = [{ id: 0, name: "Konceptualisering" }, { id: 1, name: "Konceptvalidering" }, { id: 2, name: "Produktutveckling" }, { id: 3, name: "Produktlansering" }];
  public _description: string = "";

  constructor(
    private viewService: ViewService,
    private adminService: AdminService,
    private route: ActivatedRoute,
    private alertService: AlertService,
    private router: Router
  ) {
  }

  ngOnInit() {

    this.sub = this.route.params.subscribe(params => {
      this.id = +params['id'];
      this.adminService.getThemes()
        .pipe(first())
        .subscribe(
          data => {
            this.themes = data;
            this.adminService.getBaseActivityInfo(this.id)
              .pipe(first())
              .subscribe(
                data => {
                  this.baseactivityinfo = data;
                  this._description = this.baseactivityinfo.description;
                  
                  this.editForm = new FormGroup({
                    id: new FormControl(this.baseactivityinfo.baseactivityid),
                    name: new FormControl(this.baseactivityinfo.name),
                    description: new FormControl(),
                    theme: new FormControl(this.themes.find(x => x.themeid == this.baseactivityinfo.themeid)),
                    phase: new FormControl(this.phases.find(x => x.id == this.baseactivityinfo.phase))
                  });
                  this.isDataLoaded = true;
                },
                error => {
                  const err = error.error.message || error.statusText;
                  this.alertService.error(err);
                });
          },
          error => {
          });
      //
    });
  }

  ngOnDestroy() {
  }

  editActivity() {
    this.baseactivityinfo = new ActivityInfo();
    this.baseactivityinfo.baseactivityid = this.id;
    this.baseactivityinfo.name = this.editForm.get('name').value;
    this.baseactivityinfo.description = this.editForm.get('description').value;
    this.baseactivityinfo.theme = null;
    this.baseactivityinfo.themeid = this.editForm.get('theme').value.themeid;
    this.baseactivityinfo.phase = this.editForm.get('phase').value.id;
    this.adminService.editBaseActivityInfo(this.baseactivityinfo, this.id)
      .pipe(first())
      .subscribe(
        data => {
          this.baseactivityinfo = data;
          this.alertService.success('Basaktiviteten har redigerats', { keepAfterRouteChange: true });
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
