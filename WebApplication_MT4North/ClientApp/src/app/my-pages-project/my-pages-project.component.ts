import { Component, OnDestroy } from '@angular/core';
import { ViewService, ProjectService } from '@app/_services';
import { UserProject } from '../_models';
import { first } from 'rxjs/operators';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-my-pages-project',
  templateUrl: './my-pages-project.component.html',
  styleUrls: ['./my-pages-project.component.css']
})

export class MyPagesProjectComponent implements OnDestroy {
  userProjects: UserProject[];
  userProjectsSubscription: Subscription;

  constructor(
    private viewService: ViewService,
    private projectService: ProjectService
  ) {

    this.userProjectsSubscription = this.projectService.userProjects.subscribe(x => { this.userProjects = x; console.log('selected project: ', this.userProjects[0].project.name); });
  }

  ngOnDestroy() {
    console.log('unsubscribing...');
    this.userProjectsSubscription.unsubscribe();
  }

  isFullscreen() {
    return this.viewService.isFullscreen;
  }
}
