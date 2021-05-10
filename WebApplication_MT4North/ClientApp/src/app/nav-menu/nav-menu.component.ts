import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { ViewService, AuthenticationService } from "../_services";
import { User } from '../_models';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})


export class NavMenuComponent {
  constructor(
    private viewService: ViewService,
    private router: Router,
    private authenticationService: AuthenticationService) {
    this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
  }
  currentUser: User;

  isExpanded = false;

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  isFullscreen() {
    return this.viewService.isFullscreen;
  }


  logout() {
    this.authenticationService.logout();
    this.router.navigate(['/login']);
  }
}
