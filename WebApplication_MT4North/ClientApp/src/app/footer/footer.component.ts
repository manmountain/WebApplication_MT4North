import { Component } from '@angular/core';
//import { ConnectionService } from './connection.service';
//import { FormGroup, FormBuilder, Validators } from '@angular/forms';
//import { Component, OnInit, HostListener } from '@angular/core';


@Component({
  selector: 'app-footer-component',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent {
  public currentCount = 0;

  public incrementCounter() {
    this.currentCount++;
  }
}
