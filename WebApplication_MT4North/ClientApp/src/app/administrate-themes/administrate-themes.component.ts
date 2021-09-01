import { Component, OnDestroy, OnInit, ElementRef, ViewChild } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ViewService } from "../_services";
import { AdminService, AccountService, AlertService, ProjectService } from '@app/_services';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { BehaviorSubject, Observable, throwError, Subject, Subscription } from 'rxjs';
import { first,  map, tap, catchError } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { Project, UserProject, Theme, Activity, ActivityInfo, ProjectRole, ProjectRights } from '@app/_models';
//import { AdminService } from '../_services/admin.service';


@Component({
  selector: 'app-my-pages-administrate-themes',
  templateUrl: './administrate-themes.component.html',
  styleUrls: ['./administrate-themes.component.css']
})

export class AdministrateThemes implements OnDestroy {
  isFullscreen: boolean = false;
  public themes: Theme[];

  constructor(
    private viewService: ViewService,
    private adminService: AdminService,
    private router: Router
  ) {

  }

  ngOnInit() {
    this.adminService.getThemes()
      .pipe(first())
      .subscribe(
        data => {
          console.log('AdministrateThemes ngInit data', data);
          this.themes = data;
          console.log('AdministrateThemes ngInit themes', this.themes);
        },
        error => {
          console.log('AdministrateThemes ngInit error', error);
        });
  }

  ngOnDestroy() {
    this.themes = null;
  }

  removeTheme(id: number) {
    this.adminService.deleteTheme(id)
      .pipe(first())
      .subscribe(
        data => {
          this.themes.forEach((theme, index) => {
            if (theme.themeid === data.themeid) this.themes.splice(index, 1);
          });
        },
        error => {
          console.log('AdministrateThemeEdit ngInit error', error);
        });
  }


}
