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
  selector: 'app-my-pages-administrate-users-edit',
  templateUrl: './administrate-users-edit.component.html',
  styleUrls: ['./administrate-users-edit.component.css']
})

export class AdministrateUsersEdit implements OnDestroy {
  isFullscreen: boolean = false;
  isDataLoaded: boolean = false;
  public user: User;
  public roles: string[];
  editForm: FormGroup;
  id: string;
  private sub: any;
  public gender_options: any[] = [{ id: null, name: "Vill inte ange" }, { id: 0, name: "Kvinna" }, { id: 1, name: "Man" }, { id: 2, name: "Annat" }];
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

  findRole(roleName: string): string {
    return 'AdminUser';
  }

  ngOnInit() {
    this.sub = this.route.params.subscribe(params => {
      this.id = params['id'];
      this.adminService.getRoles()
        .pipe(first())
        .subscribe(
          data => {
            this.roles = data;
            console.log('role: ', this.findRole('AdminUser'));
            
            
            this.adminService.getUser(this.id)
              .pipe(first())
              .subscribe(
                data => {
                  this.user = data;
                  console.log('user: ', data);
                  console.log('gender: ', this.gender_options.find(x => x.id == this.user.gender));
                  this.editForm = new FormGroup({
                    id: new FormControl(this.user.id),
                    firstname: new FormControl(this.user.firstname),
                    lastname: new FormControl(this.user.lastname),
                    email: new FormControl(this.user.email),
                    gender: new FormControl(this.gender_options.find(x => x.id == this.user.gender)), //(this.gender_options[0]),
                    company: new FormControl(this.user.companyname),
                    country: new FormControl(this.user.country),
                    role: new FormControl({ value: this.user.userrole, disabled: true }, Validators.required) //this.user.userrole)
                  });
                  this.isDataLoaded = true;
                },
                error => {
                  const err = error.error.message || error.statusText;
                  this.alertService.error(err);
                });
          },
          error => {
            console.log('error', error);
          });
      //
    });
  }

  ngOnDestroy() {
    this.user = null;
  }

  switchRoleToAdmin() {
    this.adminService.makeUserAdmin(this.id)
      .pipe(first())
      .subscribe(
        data => {
          //console.log('new user role ' + data.userrole + ' should be AdminUser');
          //this.alertService.success('Användarens roll har ändrats till ' + data);
          this.user = data;
        },
        error => {
          const err = error.error.message || error.statusText;
          this.alertService.error(err);
        });
  }

  switchRoleToBasic() {
    this.adminService.makeUserBasic(this.id)
      .pipe(first())
      .subscribe(
        data => {
          //console.log('new user role ' + data.userrole + ' should be BasicUser');
          //this.alertService.success('Användarens roll har ändrats till ' + data.userrole);
          this.user = data;
        },
        error => {
          const err = error.error.message || error.statusText;
          this.alertService.error(err);
        });
  }

  generateNewPassword() {
    let password = this.adminService.generatePassword();
    this.adminService.setPasswordById(this.id, password)
      .pipe(first())
      .subscribe(
        data => {
          this.newpassword = password;
        },
        error => {
          this.newpassword = '';
          console.log('error', error);
          const err = error.error.message || error.statusText;
          this.alertService.error(err);
        });


  }

  saveUser() {
    console.log('save user', this.user);
    this.user.id = this.user.id;
    this.user.firstname = this.editForm.get('firstname').value;
    this.user.lastname = this.editForm.get('lastname').value;
    this.user.email = this.editForm.get('email').value;
    this.user.country = this.editForm.get('country').value;
    this.user.gender = (this.editForm.get('gender').value.id != null) ? ("" + this.editForm.get('gender').value.id) : (this.editForm.get('gender').value.id);
    this.user.companyname = this.editForm.get('company').value;
    this.adminService.editUser(this.user, this.id)
      .pipe(first())
      .subscribe(
        data => {
          this.user = data;
          this.alertService.success('Användaren har sparats');
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
