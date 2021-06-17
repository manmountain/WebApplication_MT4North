import { Component, OnDestroy } from '@angular/core';
import { ProjectService, AccountService, AlertService } from '@app/_services';
import { User, UserProject } from '../_models';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';


@Component({
  selector: 'app-my-pages-members-component',
  templateUrl: './my-pages-members.component.html',
  styleUrls: ['./my-pages-members.component.css']
})
export class MyPagesMembersComponent implements OnDestroy {
  userProjects: UserProject[];
  currentUser: User;
  userProjectsSubscription: Subscription;
  accountSubscription: Subscription;
  hasRights = false;

  constructor(
    private projectService: ProjectService,
    private alertService: AlertService,
    private accountService: AccountService) {

    this.accountService.getCurrentUser();

    this.accountSubscription = this.accountService.currentUser.subscribe(x => { this.currentUser = x; });
    this.userProjectsSubscription = this.projectService.userProjects.subscribe(x => {
      this.userProjects = x;
      this.hasRights = (this.userProjects.filter(x => x.userid == this.currentUser.id)[0].rights == 'RW');
    });

  }

  ngOnDestroy() {
    this.userProjectsSubscription.unsubscribe();
    this.accountSubscription.unsubscribe();

  }

  updateProjectMember(userProject: UserProject) {
    console.log('updated project member data: ', userProject);
    this.projectService.updateProjectMember(userProject.userprojectid, userProject)
      .pipe(first())
      .subscribe(
        data => {
          console.log('DATA: ', data);
          this.alertService.success('Projektet har uppdaterats', { keepAfterRouteChange: true });
        },
        error => {
          this.alertService.error(error);
        });
  }
}
