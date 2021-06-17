import { Component, OnDestroy } from '@angular/core';
import { AlertService, AccountService, ProjectService } from '@app/_services';
import { User, UserProject } from '../_models';
import { first } from 'rxjs/operators';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-footer-component',
  templateUrl: './my-pages-project-settings.component.html',
  styleUrls: ['./my-pages-project-settings.component.css']
})
export class MyPagesProjectSettingsComponent implements OnDestroy {
  form: FormGroup;
  loading = false;
  submitted = false;
  hasRights = false;
  currentUser: User;
  userProjects: UserProject[];
  error = '';
  accountSubscription: Subscription;
  userProjectsSubscription: Subscription;
  editModeIsOn = false;

  constructor(
    private alertService: AlertService,
    private accountService: AccountService,
    private projectService: ProjectService,
    private formBuilder: FormBuilder
  ) {
    this.accountService.getCurrentUser();

    this.accountSubscription = this.accountService.currentUser.subscribe(x => { this.currentUser = x; });
    this.userProjectsSubscription = this.projectService.userProjects.subscribe(x => this.userProjects = x);

    console.log('**currentUser: ', this.currentUser);
    console.log('userProject: ', this.userProjects.filter(x => x.userId == this.currentUser.id));

  }

  ngOnInit() {
    this.form = this.formBuilder.group({
      projectid: [this.userProjects[0].project.projectid],
      name: [this.userProjects[0].project.name, Validators.required],
      description: [this.userProjects[0].project.description, Validators.required],
    });
    this.hasRights = (this.userProjects.filter(x => x.userId == this.currentUser.id)[0].rights == 'RW');
    console.log('user has RW rights: ', this.hasRights);
  }

  ngOnDestroy() {
    console.log('unsubscribing from observers in project settings');
    this.accountSubscription.unsubscribe();
    this.userProjectsSubscription.unsubscribe();
  }

  activateEditMode(param: boolean) {
    this.editModeIsOn = param;
    console.log('edit mode on? ', param);
  }

  // convenience getter for easy access to form fields
  get f() { return this.form.controls; }

  onSubmit() {
    console.log('form submitted');
    this.submitted = true;

    // reset alerts on submit
    this.alertService.clear();

    // stop here if form is invalid
    if (this.form.invalid) {
      return;
    }
    console.log('form value: ', this.form.value);

    this.loading = true;
    this.projectService.update(this.form.value)
      .pipe(first())
      .subscribe(
        data => {
          this.alertService.success('Projektet har uppdaterats', { keepAfterRouteChange: true });
          this.loading = false;
        },
        error => {
          this.alertService.error(error);
          this.loading = false;
        });
  }
}
