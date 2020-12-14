import { Component } from '@angular/core';
import { Theme } from "../models/theme.model";
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-my-pages-activity-status',
  templateUrl: './my-pages-activity-status.component.html',
  styleUrls: ['./my-pages-activity-status.component.css']
})

export class MyPagesActivityStatusComponent {
  headers = ["Namn", "Konceptualisering", "Proof of concept", "Produktvalidering", "Produktlansering"];
  themes: Theme[];

  rows = [
    {
      "Namn": "Tema 1",
      "Konceptualisering": [{ "Namn": "Aktivitet 1", "Beskrivning": "Beskrivning" }, { "Namn": "Aktivitet 1", "Beskrivning": "Beskrivning" }],
      "Proof of concept": "21",
      "Produktvalidering": "Male",
      "Produktlansering": "India"
    },
    {
      "Namn": "Tema 2",
      "Konceptualisering": "Ajay",
      "Proof of concept": "21",
      "Produktvalidering": "Male",
      "Produktlansering": "India"
    }
  ];


}
