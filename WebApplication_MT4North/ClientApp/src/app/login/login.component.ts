import { sharedStylesheetJitUrl } from '@angular/compiler';
import { Component } from '@angular/core';

export class Student {
  public name: string;
  public email: string;
  public password: string;
  public subjects: string;
}

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
 
})
export class LoginComponent {
  model = new Student();

  submit(data) {
    console.log(data.value)
  }
}
