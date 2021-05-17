import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';

import { AccountService, AlertService } from '@app/_services';

@Component({ templateUrl: 'register.component.html' })
export class RegisterComponent implements OnInit {
  form: FormGroup;
  loading = false;
  submitted = false;

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private accountService: AccountService,
    private alertService: AlertService
  ) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      //username: ['', Validators.required],
      email: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(8), Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&].{8,}')]]
    });
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
    this.accountService.register(this.form.value)
      .pipe(first())
      .subscribe(
        data => {
          this.alertService.success('Registreringen lyckades', { keepAfterRouteChange: true });
          this.router.navigate(['../login'], { relativeTo: this.route });
        },
        error => {
          this.alertService.error(error);
          this.loading = false;
        });
  }
}

//import { Component, OnInit } from '@angular/core';
//import { FormControl, FormGroup, Validators  } from "@angular/forms";

//export class Student {
//  public name: string;
//  public email: string;
//  public password: string;
//  public subjects: string;
//}

//@Component({
//  selector: 'app-register',
//  templateUrl: './register.component.html',
//  styleUrls: ['./register.component.css']
//})
//export class RegisterComponent implements OnInit {
//  model = new Student();

//  Subjects: string[] = [
//    'Science',
//    'Math',
//    'Physics',
//    'Finance'
//  ];


//  ngOnInit() { }

//  constructor() { }

//  submit(data) {
//    console.log(data.value)
//  }

//}
