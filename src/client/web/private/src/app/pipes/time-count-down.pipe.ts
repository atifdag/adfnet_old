import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timeCountDown'
})
export class TimeCountDownPipe implements PipeTransform {

  transform(value: number): string {
    const minutes: number = Math.floor(value / 60);
    return ('00' + minutes).slice(-2) + ':' + ('00' + Math.floor(value - minutes * 60)).slice(-2);
  }

}
