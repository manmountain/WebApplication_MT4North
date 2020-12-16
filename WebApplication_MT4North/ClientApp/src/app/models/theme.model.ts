import { Activity } from "./activity.model";
import { Phase } from "./common";

export class Theme {
  name: string;
  description: string;
  activities: Activity[] = [];

  constructor(name: string, description: string) {
    this.name = name;
    this.description = description;
  }

  addActivity(activity: Activity) {
    console.log("activity name in constr: ", activity.name);
    this.activities.push(activity);
  }

  /**
   
  filterActivitiesBasedOnPhase(activity):Activity[] {
    return this.activities.find(x => x.phase == phase);
  }
  */
    /**
      Fredrik was here
     */
}
