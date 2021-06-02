import { Component } from '@angular/core';
import { ViewService, AlertService, ProjectService } from '@app/_services';
import { User, Project } from '../_models';
import { first } from 'rxjs/operators';

@Component({
  selector: 'app-my-pages-project',
  templateUrl: './my-pages-project.component.html',
  styleUrls: ['./my-pages-project.component.css']
})

export class MyPagesProjectComponent {
  currentUser: User;
  project: Project;
  error = '';

  constructor(
    private alertService: AlertService,
    private viewService: ViewService,
    private projectService: ProjectService
  ) {
    //this.projectService.getProjects()
    //  .pipe(first())
    //  .subscribe(
    //    data => { },

    //    error => {
    //      this.error = error;
    //      this.alertService.error(error);
    //    });
    //this.projectService.projects.subscribe(x => this.project = x);

  }

  isFullscreen() {
    return this.viewService.isFullscreen;
  }
}
