import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoggerService {

  constructor() { }

  static log(message: string) {
    console.log(message);
  }
  
  static logObj(object: any) {
    console.log(object);
  }
}
