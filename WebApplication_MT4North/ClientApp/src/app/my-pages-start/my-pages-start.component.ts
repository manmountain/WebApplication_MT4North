import { Component } from '@angular/core';
import { AccountService } from '@app/_services';

@Component({
  selector: 'app-my-pages-start',
  templateUrl: './my-pages-start.component.html',
  styleUrls: ['./my-pages-start.component.css']
})

export class MyPagesStartComponent {
  currentUser = null;
  constructor(
    private accountService: AccountService
  ) {
    this.currentUser = accountService.currentUserValue;
  }
}
