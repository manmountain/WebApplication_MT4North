import { Component, OnDestroy } from '@angular/core';
import { AccountService, ProjectService } from '@app/_services';
import { Subscription } from 'rxjs';
import { User, UserProject, ProjectRights, ProjectRole } from '../_models';
import { first } from 'rxjs/operators';


@Component({
  selector: 'app-my-pages-start',
  templateUrl: './my-pages-start.component.html',
  styleUrls: ['./my-pages-start.component.css']
})

export class MyPagesStartComponent implements OnDestroy {
  permissions = ProjectRights;
  roles = ProjectRole;
  currentUser: User;
  invitations: UserProject[];
  accountSubscription: Subscription;
  invitationsSubscription: Subscription;
  error = '';

  constructor(
    private accountService: AccountService,
    private projectService: ProjectService
  ) {
    this.accountSubscription = this.accountService.currentUser.subscribe(x => {this.currentUser = x;});
    this.accountService.getCurrentUser()
      .pipe(first())
      .subscribe(
        data => { },

        error => {
          const err = error.error.message || error.statusText;
          this.error = err;
          //this.alertService.error(err);
        });

    this.invitationsSubscription = this.projectService.invitations.subscribe(x => { this.invitations = x; });
    this.projectService.getInvites()
      .pipe(first())
      .subscribe(
        data => {
          console.log('invites: ', data);
        },

        error => {
          const err = error.error.message || error.statusText;
          console.log('error: ', err);

          this.error = err;
          //this.alertService.error(err);
        });

    console.log('invitations: ', this.invitations);
  }

  ngOnDestroy() {
    this.accountSubscription.unsubscribe();
    this.invitationsSubscription.unsubscribe();
  }

  acceptInvite(userProjectId: string) {
    this.projectService.acceptInvite(userProjectId)
      .pipe(first())
      .subscribe(
        data => {
          console.log('invitation accepted: ', data);
          let elementToRemove = this.invitations.filter(x => x.userprojectid == data.userprojectid)[0];
          let idToRemove = this.invitations.indexOf(elementToRemove, 0);
          if (idToRemove > -1) {
            this.invitations.splice(idToRemove, 1);
          }
          console.log('INVITATIO LEFT AFTER REMOVE: ', this.invitations);
          //this.alertService.success('Ditt konto har uppdaterats', { keepAfterRouteChange: true });
          //this.loading = false;
        },
        error => {
          const err = error.error.message || error.statusText;
          console.log('invitation accepted not working: ', err);

          //this.alertService.error(err);
          //this.loading = false;
        });
  }

  rejectInvite(userProjectId: string) {
    this.projectService.rejectInvite(userProjectId)
      .pipe(first())
      .subscribe(
        data => {
          console.log('invitation rejected: ', data);
          let elementToRemove = this.invitations.filter(x => x.userprojectid == data.userprojectid)[0];
          let idToRemove = this.invitations.indexOf(elementToRemove, 0);
          if (idToRemove > -1) {
            this.invitations.splice(idToRemove, 1);
          }
          console.log('INVITATION LEFT AFTER REMOVE: ', this.invitations);

          //this.alertService.success('Ditt konto har uppdaterats', { keepAfterRouteChange: true });
          //this.loading = false;
        },
        error => {
          const err = error.error.message || error.statusText;
          console.log('invitation rejected not working: ', err);

          //this.alertService.error(err);
          //this.loading = false;
        });
  }
}
