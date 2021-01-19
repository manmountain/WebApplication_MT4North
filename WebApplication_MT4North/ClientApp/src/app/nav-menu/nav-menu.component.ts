import { Component } from '@angular/core';
import { ViewService } from "../view.service";

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})


export class NavMenuComponent {
  constructor(private viewService: ViewService) {
  }

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
}
