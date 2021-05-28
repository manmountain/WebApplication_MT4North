import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { ViewService, AccountService } from "../_services";
import { HostListener } from '@angular/core';

import { User } from '../_models';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})


export class NavMenuComponent {
  selectedNavItem = "Home";

  constructor(
    private viewService: ViewService,
    private router: Router,
    private accountService: AccountService) {
    this.accountService.currentUser.subscribe(x => this.currentUser = x);
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

  makeActive(navItem: string) {
    this.selectedNavItem = navItem;
  }

  logout() {
    this.accountService.logout();
    this.router.navigate(['/login']);
  }

  @HostListener('window:scroll', ['$event'])

  onWindowScroll() {
    let element = document.querySelector('.navbar') as HTMLElement;
    if (window.pageYOffset > element.clientHeight) {
      element.classList.add('navbar-inverse');
    } else {
      element.classList.remove('navbar-inverse');
    }
  }
}
