import { Phase } from "./common";
import { Status } from "./common";


export class Activity {
  name: string;
  description: string;
  phase: Phase;
  status: Status;
  isExcluded: Boolean;

  constructor(name: string, description: string, phase: Phase, isExcluded: Boolean) {
    this.name = name;
    this.description = description;
    this.phase = phase;
    this.status = Status.NOTSTARTED;
    this.isExcluded = isExcluded;
  }
}
