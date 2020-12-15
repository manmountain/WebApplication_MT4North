import { Inject } from "@angular/core";
import { Phase } from "./common";


export class Activity {
  name: string;
  description: string;
  phase: Phase;

  constructor(name: string, description: string, phase: Phase) {
    this.name = name;
    this.description = description;
    this.phase = phase;

    console.log("pahe : ", phase);
  }
}
