import { Component, OnDestroy, OnInit, ElementRef, ViewChild } from '@angular/core';
import { ViewService } from "../_services";
import { AccountService, AlertService } from '@app/_services';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { first } from 'rxjs/operators';
import { User, Alert, AlertType } from '../_models';
import { Subscription } from 'rxjs';;


@Component({
  selector: 'app-my-pages-edit-account',
  templateUrl: './my-pages-edit-account.component.html',
  styleUrls: ['./my-pages-edit-account.component.css']
})

export class MyPagesEditAccountComponent implements OnDestroy {
  form: FormGroup;
  loading = false;
  submitted = false;
  currentUser: User;
  accountSubscription = Subscription;

  constructor(
    private viewService: ViewService,
    private accountService: AccountService,
    private alertService: AlertService,
    private formBuilder: FormBuilder,

  ) {
    this.accountSubscription = this.accountService.currentUser.subscribe(x => this.currentUser = x);
  }

  ngOnInit() {
    this.form = this.formBuilder.group({
      firstname: [this.currentUser.firstname, Validators.required],
      lastname: [this.currentUser.lastname, Validators.required],
      email: [this.currentUser.email, [Validators.required, Validators.email]],
    });

  }

  ngOnDestroy() {
    this.accountSubscription.unsubscribe();
  }

  isFullscreen() {
    return this.viewService.isFullscreen;
  }


  // convenience getter for easy access to form fields
  get f() { return this.form.controls; }

  onSubmit() {
    this.submitted = true;

    // reset alerts on submit
    this.alertService.clear();

    // stop here if form is invalid
    if (this.form.invalid) {
      return;
    }

    this.loading = true;
    this.accountService.update(this.form.value)
      .pipe(first())
      .subscribe(
        data => {
          this.alertService.success('Ditt konto har uppdaterats', { keepAfterRouteChange: true });
          this.loading = false;
        },
        error => {
          this.alertService.error(error);
          this.loading = false;
        });
  }
}
