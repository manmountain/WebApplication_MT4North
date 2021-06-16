import { Component, OnDestroy } from '@angular/core';
import { AccountService, ProjectService } from '@app/_services';
import { Subscription } from 'rxjs';
import { User } from '../_models';
import { first } from 'rxjs/operators';


@Component({
  selector: 'app-my-pages-start',
  templateUrl: './my-pages-start.component.html',
  styleUrls: ['./my-pages-start.component.css']
})

export class MyPagesStartComponent implements OnDestroy {
  currentUser: User;
  accountSubscription: Subscription;
  error = '';

  constructor(
    private accountService: AccountService,
    private projectService: AccountService
  ) {
    this.accountSubscription = this.accountService.currentUser.subscribe(x => { this.currentUser = x; });
    this.accountService.getCurrentUser()
      .pipe(first())
      .subscribe(
        data => { },

        error => {
          this.error = error;
          //this.alertService.error(error);
        });
  }

  ngOnDestroy() {
    this.accountSubscription.unsubscribe();
  }
}
