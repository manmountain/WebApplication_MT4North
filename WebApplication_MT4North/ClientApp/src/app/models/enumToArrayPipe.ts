import { PipeTransform, Pipe } from '@angular/core';
import { Activity } from "./activity.model";

import { Phase } from "./common";

@Pipe({
  name: 'enumToArray',
  pure: false
})
export class EnumToArrayPipe implements PipeTransform {
  transform(data: Object) {
    return Object.values(data);;
  }
}
