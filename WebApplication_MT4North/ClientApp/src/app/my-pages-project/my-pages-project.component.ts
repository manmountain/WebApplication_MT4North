import { Component } from '@angular/core';
import { ViewService } from "../_services";

@Component({
  selector: 'app-my-pages-project',
  templateUrl: './my-pages-project.component.html',
  styleUrls: ['./my-pages-project.component.css']
})

export class MyPagesProjectComponent {
  constructor(private viewService: ViewService) {
  }

  isFullscreen() {
    return this.viewService.isFullscreen;
  }
}
