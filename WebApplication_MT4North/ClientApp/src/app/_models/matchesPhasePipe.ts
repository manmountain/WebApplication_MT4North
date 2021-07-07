import { PipeTransform, Pipe } from '@angular/core';
import { Activity } from "./activity.model";
import { ActivityPhase } from "./enums";


@Pipe({
  name: 'matchesPhase',
  pure: false
})
export class MatchesPhasePipe implements PipeTransform {
  transform(activities: Activity[], phase: ActivityPhase): Activity[] {
    if (!activities)
      return [];
    if (!phase) return activities;
    
    return activities.filter(item => item.baseactivityinfo != null && item.baseactivityinfo.phase === phase);
  }
}
