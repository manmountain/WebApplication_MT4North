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
  projectNameIsEditable = false;
  projectDescriptionIsEditable = false;

  constructor(
    private alertService: AlertService,
    private accountService: AccountService,
    private projectService: ProjectService,
    private formBuilder: FormBuilder
  ) {
    this.accountService.getCurrentUser();

    this.accountSubscription = this.accountService.currentUser.subscribe(x => { this.currentUser = x; });
    this.userProjectsSubscription = this.projectService.selectedProject.subscribe(x => this.userProjects = x);

    console.log('**currentUser: ', this.currentUser);
    console.log('userProject: ', this.userProjects.filter(x => x.userId == this.currentUser.id));

    this.hasRights = (this.userProjects.filter(x => x.userId == this.currentUser.id)[0].rights == 'RW');
    console.log('user has RW rights: ', this.hasRights);
  }

  ngOnInit() {
    this.form = this.formBuilder.group({
      projectName: [this.userProjects[0].project.name, Validators.required],
      projectDescription: [this.userProjects[0].project.description, Validators.required],
    });
  }

  ngOnDestroy() {
    this.accountSubscription.unsubscribe();
    this.userProjectsSubscription.unsubscribe();
  }

  editProjectName(param: boolean) {
    this.projectNameIsEditable = param;
  }

  editProjectDescription(param: boolean) {
    this.projectDescriptionIsEditable = param;
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
