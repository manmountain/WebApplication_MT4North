import { Component } from '@angular/core';
import { AlertService, AccountService, ProjectService } from '@app/_services';
import { User, Project } from '../_models';
import { first } from 'rxjs/operators';


@Component({
  selector: 'app-footer-component',
  templateUrl: './my-pages-project-settings.component.html',
  styleUrls: ['./my-pages-project-settings.component.css']
})
export class MyPagesProjectSettingsComponent {
  currentUser: User;
  projects: Project[];
  error = '';

  constructor(
    private alertService: AlertService,
    private accountService: AccountService,
    private projectService: ProjectService
  ) {
    this.accountService.currentUser.subscribe(x => { this.currentUser = x; });
    this.projectService.getProjects()
      .pipe(first())
      .subscribe(
        data => { },

        error => {
          this.error = error;
          this.alertService.error(error);
        });
    this.projectService.projects.subscribe(x => this.projects = x);

  }
}
