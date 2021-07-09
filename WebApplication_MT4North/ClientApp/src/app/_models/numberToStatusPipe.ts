import { PipeTransform, Pipe } from '@angular/core';
import { ActivityStatus } from './enums';

@Pipe({
  name: 'numberToStatus',
  pure: false
})
export class NumberToStatusPipe implements PipeTransform {
  transform(input: ActivityStatus): string { //string type
    switch (input) {
      case ActivityStatus.NOTSTARTED: {
        return "unchecked";
      }
      case ActivityStatus.ONGOING: {
        return "crossed";
      }
      case ActivityStatus.FINISHED: {
        return "checked";
      }
    }
  }
}
