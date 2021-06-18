import { Component, OnDestroy } from '@angular/core';
import { ViewService, ProjectService } from '@app/_services';
import { Project } from '../_models';
import { first } from 'rxjs/operators';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-my-pages-project',
  templateUrl: './my-pages-project.component.html',
  styleUrls: ['./my-pages-project.component.css']
})

export class MyPagesProjectComponent implements OnDestroy {
  selectedProject: Project;
  selectedProjectSubscription: Subscription;

  constructor(
    private viewService: ViewService,
    private projectService: ProjectService
  ) {

    this.selectedProjectSubscription = this.projectService.selectedProject.subscribe(x => { this.selectedProject = x; });
  }

  ngOnDestroy() {
    console.log('unsubscribing...');
    this.selectedProjectSubscription.unsubscribe();
  }

  isFullscreen() {
    return this.viewService.isFullscreen;
  }
}
