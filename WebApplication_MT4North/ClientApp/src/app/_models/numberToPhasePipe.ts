import { PipeTransform, Pipe } from '@angular/core';
import { ActivityPhase } from './enums';

@Pipe({
  name: 'numberToPhase',
  pure: false
})
export class NumberToPhasePipe implements PipeTransform {
  transform(input: ActivityPhase): string { //string type
    switch (input) {
      case ActivityPhase.CONCEPTUALIZATION: {
        return "Konceptualisering";
      }
      case ActivityPhase.VALIDATION: {
        return "Konceptvalidering";
      }
      case ActivityPhase.DEVELOPMENT: {
        return "Produktutveckling";
      }
      case ActivityPhase.LAUNCH: {
        return "Produktlansering";
      }
    }
  }
}
