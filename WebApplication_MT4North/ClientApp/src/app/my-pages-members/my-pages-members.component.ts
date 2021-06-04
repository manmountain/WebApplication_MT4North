import { Component } from '@angular/core';
import { ProjectService } from '@app/_services';
import { UserProject } from '../_models';


@Component({
  selector: 'app-my-pages-members-component',
  templateUrl: './my-pages-members.component.html',
  styleUrls: ['./my-pages-members.component.css']
})
export class MyPagesMembersComponent {
  userProjects: UserProject[];

  constructor(private projectService: ProjectService) {

    this.projectService.selectedProject.subscribe(x => this.userProjects = x);
  }
}
