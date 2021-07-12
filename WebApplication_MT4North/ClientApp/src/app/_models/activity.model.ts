import { ActivityInfo, Note } from '@app/_models';
import { ActivityStatus } from "./enums";


export class Activity {
  activityid: string;
  isexcluded: Boolean;
  status: ActivityStatus;
  startdate: string;
  finishdate: string;
  deadlinedate: string;
  resources: string;
  projectId: string;
  baseactivityinfoid: string;
  customactivityinfoid: string;
  baseactivityinfo: ActivityInfo;
  customactivityinfo: ActivityInfo;
  notes: Note[];
}
