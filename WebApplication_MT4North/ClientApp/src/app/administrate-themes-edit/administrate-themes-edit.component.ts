import { Component, OnDestroy, OnInit, ElementRef, ViewChild } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ViewService } from "../_services";
import { AdminService, AccountService, AlertService, ProjectService } from '@app/_services';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { BehaviorSubject, Observable, throwError, Subject, Subscription } from 'rxjs';
import { first,  map, tap, catchError } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { Project, UserProject, Theme, Activity, ActivityInfo, ProjectRole, ProjectRights } from '@app/_models';
//import { AdminService } from '../_services/admin.service';


@Component({
  selector: 'app-my-pages-administrate-themes-edit',
  templateUrl: './administrate-themes-edit.component.html',
  styleUrls: ['./administrate-themes-edit.component.css']
})

export class AdministrateThemesEdit implements OnDestroy {
  isFullscreen: boolean = false;
  isDataLoaded: boolean = false;
  editForm: FormGroup;
  id: number;
  private sub: any;
  public theme: Theme;

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
      this.adminService.getTheme(this.id)
        .pipe(first())
        .subscribe(
          data => {
            this.theme = data;
            this.isDataLoaded = true;
          },
          error => {
            const err = error.error.message || error.statusText;
            this.alertService.error(err);
          });
    });
    this.editForm = new FormGroup({
      id: new FormControl({disabled: true}),
      name: new FormControl(),
      description: new FormControl()
    });
  }

  ngOnDestroy() {
    this.theme = null;
  }

  editTheme() {
    //this.theme.themeid = this.editForm.get('id').value;
    this.theme.name = this.editForm.get('name').value;
    this.theme.description = this.editForm.get('description').value;
    this.adminService.editTheme(this.theme, this.id)
      .pipe(first())
      .subscribe(
        data => {
          this.theme = data;
          this.alertService.success('Basaktiviteten har redigerats', { keepAfterRouteChange: true });
          this.router.navigate(['/my-pages/administrate-themes']);
        },
        error => {
          const err = error.error.message || error.statusText;
          this.alertService.error(err);
        });
  }

  back() {
    this.router.navigate(['/my-pages/administrate-themes']);
  }

}
