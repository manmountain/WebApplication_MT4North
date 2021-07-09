import { PipeTransform, Pipe } from '@angular/core';
import { ProjectRights } from './enums';

@Pipe({
  name: 'numberToRights',
  pure: false
})
export class NumberToRightsPipe implements PipeTransform {
  transform(input: ProjectRights): string { //string type
    switch (input) {
      case ProjectRights.READ: {
        return "Kan endast läsa";
      }
      case ProjectRights.WRITE: {
        return "Kan endast redigera";
      }
      case ProjectRights.READWRITE: {
        return "Kan läsa och redigera";
      }
    }
  }
}
