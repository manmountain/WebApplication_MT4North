import { PipeTransform, Pipe } from '@angular/core';
import { Theme, Activity } from "@app/_models";
import { ActivityPhase } from'./enums';


@Pipe({
  name: 'matchesPhaseAndTheme',
  pure: false
})
export class MatchesPhaseAndThemePipe implements PipeTransform {
  transform(activities: Activity[], phase: number, theme: Theme): Activity[] {
    if (!activities)
      return [];
    //if (!phase) {
    //  console.log('all activities returned, phase: ', phase);
    //  return [];
    //}

    let baseActivities = activities.filter(item => item.baseactivityinfo != null).filter(item => item.baseactivityinfo.themeid == theme.themeid && item.baseactivityinfo.phase === phase);
    let customActivities = activities.filter(item => item.customactivityinfo != null).filter(item => item.customactivityinfo.themeid == theme.themeid && item.customactivityinfo.phase === phase);

    return baseActivities.concat(customActivities);

    //return filteredActivities.sort(x => {
    //  return x.baseactivityinfoid == null ? 1 : -1 // `false` values first
    //})
    //return activities.filter(item => item.baseactivityinfo != null).filter(item => item.baseactivityinfo.themeid == theme.themeid && item.baseactivityinfo.phase === phase);
  }
}
