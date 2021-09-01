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
  selector: 'app-my-pages-administrate-themes-new',
  templateUrl: './administrate-themes-new.component.html',
  styleUrls: ['./administrate-themes-new.component.css']
})

export class AdministrateThemesNew implements OnDestroy {
  isFullscreen: boolean = false;
  isDataLoaded: boolean = false;
  newForm: FormGroup;
  private sub: any;
  public theme: Theme;

  constructor(
    private viewService: ViewService,
    private adminService: AdminService,
    private alertService: AlertService,
    private router: Router
  ) {

  }

  ngOnInit() {
    this.newForm = new FormGroup({
      id: new FormControl({ disabled: true }),
      name: new FormControl(),
      description: new FormControl()
    });
    this.theme = new Theme("", "");
    this.isDataLoaded = true;
  }

  newTheme() {
    //this.theme = new Theme(this.newForm.get('name').value, this.newForm.get('description').value);
    this.theme.name = this.newForm.get('name').value;
    this.theme.description = this.newForm.get('description').value;
    this.adminService.createTheme(this.theme)
      .pipe(first())
      .subscribe(
        data => {
          this.theme = data;
          this.alertService.success('Basaktiviteten har skapats', { keepAfterRouteChange: true });
          this.router.navigate(['/my-pages/administrate-themes']);
        },
        error => {
          const err = error.error.message || error.statusText;
          this.alertService.error(err);
        });
  }

  ngOnDestroy() {
    this.theme = null;
  }

  back() {
    this.router.navigate(['/my-pages/administrate-themes']);
  }

}
