import { Pipe, PipeTransform } from '@angular/core';
import { Exchange } from '../models/Exchange';

@Pipe({
  name: 'filter'
})
export class FilterPipe implements PipeTransform {

  transform(value: any, args?: any): any {
    let exchanges = value as Exchange[];
    let filterString = args;

    if (filterString === undefined || (filterString !== undefined && filterString.length === 0))
      return exchanges;

    if (filterString !== undefined)
      filterString = filterString.toLowerCase();

    return exchanges.filter(exchange => exchange.name.toLowerCase().indexOf(filterString) !== -1);
  }
}
