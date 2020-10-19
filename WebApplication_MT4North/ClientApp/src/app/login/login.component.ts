import { Component} from '@angular/core';

@Component({
  selector: 'login',
  templateUrl: './login.component.html'
})
export class LoginComponent {

   public currentCount = 1000;

  public decrementCount() {
    this.currentCount--;
  } 
    
}
