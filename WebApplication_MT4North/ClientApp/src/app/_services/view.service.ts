import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ViewService {
  isFullscreen: boolean = false;

  constructor() { }
}
