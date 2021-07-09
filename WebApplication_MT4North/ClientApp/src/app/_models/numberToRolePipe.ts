import { PipeTransform, Pipe } from '@angular/core';
import { ProjectRole } from './enums';

@Pipe({
  name: 'numberToRole',
  pure: false
})
export class NumberToRolePipe implements PipeTransform {
  transform(input: ProjectRole): string { //string type
    switch (input) {
      case ProjectRole.OWNER: {
        return "Projektägare";
      }
      case ProjectRole.PARTICIPANT: {
        return "Projektdeltagare";
      }
    }
  }
}
