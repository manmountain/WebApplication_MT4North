import { PipeTransform, Pipe } from '@angular/core';

@Pipe({
  name: 'jsonToDate',
  pure: false
})
export class JsonToDatePipe implements PipeTransform {
  transform(input: string): string { //string type
    //const options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };

    return new Date(input).toLocaleDateString('se-SE');
  }
}
