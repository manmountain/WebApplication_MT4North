import { Component } from '@angular/core';
import { ViewService, ProjectService } from '@app/_services';
import { UserProject } from '../_models';
import { first } from 'rxjs/operators';

@Component({
  selector: 'app-my-pages-project',
  templateUrl: './my-pages-project.component.html',
  styleUrls: ['./my-pages-project.component.css']
})

export class MyPagesProjectComponent {
  userProjects: UserProject[];

  constructor(
    private viewService: ViewService,
    private projectService: ProjectService
  ) {

    this.projectService.selectedProject.subscribe(x => this.userProjects = x);
  }

  isFullscreen() {
    return this.viewService.isFullscreen;
  }
}
