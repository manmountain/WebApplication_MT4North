import { Component } from '@angular/core';
//import { ConnectionService } from './connection.service';
//import { FormGroup, FormBuilder, Validators } from '@angular/forms';
//import { Component, OnInit, HostListener } from '@angular/core';


@Component({
  selector: 'app-counter-component',
  templateUrl: './counter.component.html',
  styleUrls: ['./counter.component.css']
})
export class CounterComponent {
  public currentCount = 0;

  public incrementCounter() {
    this.currentCount++;
  }
}




