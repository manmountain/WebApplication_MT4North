import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators  } from "@angular/forms";

export class Student {
  public name: string;
  public email: string;
  public password: string;
  public subjects: string;
}

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model = new Student();

  Subjects: string[] = [
    'Science',
    'Math',
    'Physics',
    'Finance'
  ];


  ngOnInit() { }

  constructor() { }

  submit(data) {
    console.log(data.value)
  }

}
