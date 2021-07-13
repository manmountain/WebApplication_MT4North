import { ActivityInfo, Note, Project } from '@app/_models';
import { ActivityStatus } from "./enums";


export class Activity {
  activityid: number;
  isexcluded: Boolean;
  status: ActivityStatus;
  startdata: string;
  finishdate: string;
  deadlinedate: string;
  resources: string;
  projectid: string;
  baseactivityinfoid: number;
  customactivityinfoid: number;
  baseactivityinfo: ActivityInfo;
  customactivityinfo: ActivityInfo;
  notes: Note[];
  project: Project;

  constructor() {
    this.activityid = 0;
    this.isexcluded = false;
    this.status = ActivityStatus.NOTSTARTED;
    this.startdata = null;
    this.finishdate = null;
    this.deadlinedate = null;
    this.resources = null;
    this.projectid = null;
    this.baseactivityinfoid = null;
    this.customactivityinfoid = null;
    this.baseactivityinfo = null;
    this.customactivityinfo = null;
    this.notes = [];
    this.project = null;
  }
}
