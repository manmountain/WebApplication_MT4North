import { Activity } from "./activity.model";

export class Theme {
  name: string;
  description: string;
  activities: Activity[] = [];

  constructor(name: string, description: string) {
    this.name = name;
    this.description = description;
  }

  addActivity(activity: Activity) {
    this.activities.push(activity);
  }

  /**
   
  filterActivitiesBasedOnPhase(activity):Activity[] {
    return this.activities.find(x => x.phase == phase);
  }
  */
  /**
   * testing
   * */
}
