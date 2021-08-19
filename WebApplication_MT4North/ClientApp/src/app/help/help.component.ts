import { Component, OnInit } from '@angular/core';

@Component({
  templateUrl: 'help.component.html',
  styleUrls: ['./help.component.css']})
export class HelpComponent implements OnInit {
  selectedItem = "section1";

  constructor(
  ) { }

  ngOnInit() {

  }

  listClick(event, newValue) {
    console.log(newValue);
    this.selectedItem = newValue;  // don't forget to update the model here
    // ... do other stuff here ...
  }

}
