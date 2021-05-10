import { PipeTransform, Pipe } from '@angular/core';
import { Activity } from "./activity.model";

import { Phase } from "./common";

@Pipe({
  name: 'matchesPhase',
  pure: false
})
export class MatchesPhasePipe implements PipeTransform {
  transform(activities: Activity[], phase: string): Activity[] {
    if (!activities)
      return [];
    if (!phase) return activities;

    return activities.filter(item => item.phase === phase);
  }
}
