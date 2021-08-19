//import { Component } from '@angular/core';

//@Component({
//  selector: 'app-home',
//  templateUrl: './home.component.html',
//  styleUrls: ['./home.component.css'],
//})
//export class HomeComponent {
//}

import { Component } from '@angular/core';
import { first } from 'rxjs/operators';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { User } from '../_models';
import { UserService, AccountService } from '../_services';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  templateUrl: 'home.component.html',
  styleUrls: ['home.component.css']
})

export class HomeComponent {
    loading = false;
  users: User[];
  email: '';
  form: FormGroup;
  submitted = false;

  constructor(
    private userService: UserService,
    private formBuilder: FormBuilder,
    private router: Router,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.loading = true;

    this.form = this.formBuilder.group({
      email: ['', Validators.email]
    });
    //this.userService.getAll().pipe(first()).subscribe(users => {
    //    this.loading = false;
    //    this.users = users;
    //});
  }

  onSubmit() {
    this.submitted = true;
    if (this.form.invalid && this.email!='') {
      return
    }

    this.router.navigate(['../register'], { queryParams: { email: this.email } });

  }

  // convenience getter for easy access to form fields
  get f() { return this.form.controls; }
}
