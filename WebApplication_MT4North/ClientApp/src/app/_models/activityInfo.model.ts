import { ActivityPhase, Theme } from "../_models";

export class ActivityInfo {
  customactivityid: number;
  baseactivityid: number;
  name: string;
  description: string;
  phase: ActivityPhase;
  themeid: string;
  theme: Theme;

  constructor() {
    this.customactivityid = null;
    this.baseactivityid = null;
    this.name = null;
    this.description = null;
    this.phase = null;
    this.themeid = null;
    this.theme = null;
  }
}
