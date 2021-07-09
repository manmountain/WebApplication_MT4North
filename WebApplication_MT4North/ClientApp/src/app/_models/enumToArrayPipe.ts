import { PipeTransform, Pipe } from '@angular/core';

@Pipe({
  name: 'enumToArray',
  pure: false
})
export class EnumToArrayPipe implements PipeTransform {
  transform(data: Object) {
    if (!data) {
      return [];
    }
    //return Object.values(data);;
    const values = Object.values(data);
    return values.slice(values.length / 2);
  }
}
