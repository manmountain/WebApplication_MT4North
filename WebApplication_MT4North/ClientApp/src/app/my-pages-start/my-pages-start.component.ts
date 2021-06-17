import { Component, OnDestroy } from '@angular/core';
import { AccountService, ProjectService } from '@app/_services';
import { Subscription } from 'rxjs';
import { User, UserProject } from '../_models';
import { first } from 'rxjs/operators';


@Component({
  selector: 'app-my-pages-start',
  templateUrl: './my-pages-start.component.html',
  styleUrls: ['./my-pages-start.component.css']
})

export class MyPagesStartComponent implements OnDestroy {
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
          this.error = error;
          //this.alertService.error(error);
        });

    this.invitationsSubscription = this.projectService.invitations.subscribe(x => { this.invitations = x; });
    this.projectService.getInvites()
      .pipe(first())
      .subscribe(
        data => {
          console.log('invites: ', data);
        },

        error => {
          console.log('error: ', error);

          this.error = error;
          //this.alertService.error(error);
        });

    console.log('invitations: ', this.invitations);
  }

  ngOnDestroy() {
    this.accountSubscription.unsubscribe();
    this.invitationsSubscription.unsubscribe();

  }
}
