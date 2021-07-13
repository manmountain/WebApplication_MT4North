import { PipeTransform, Pipe } from '@angular/core';

@Pipe({
  name: 'jsonToDate',
  pure: false
})
export class JsonToDatePipe implements PipeTransform {
  transform(input: string): string { //string type
    return new Date(input).toDateString();
  }
}
