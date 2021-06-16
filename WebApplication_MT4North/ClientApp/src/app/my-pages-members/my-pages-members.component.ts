import { Component, OnDestroy } from '@angular/core';
import { ProjectService } from '@app/_services';
import { UserProject } from '../_models';
import { Subscription } from 'rxjs';


@Component({
  selector: 'app-my-pages-members-component',
  templateUrl: './my-pages-members.component.html',
  styleUrls: ['./my-pages-members.component.css']
})
export class MyPagesMembersComponent implements OnDestroy {
  userProjects: UserProject[];
  userProjectsSubscription: Subscription;

  constructor(private projectService: ProjectService) {

    this.userProjectsSubscription = this.projectService.userProjects.subscribe(x => this.userProjects = x);
  }

  ngOnDestroy() {
    this.userProjectsSubscription.unsubscribe();
  }
}
