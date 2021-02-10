import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse, HttpHeaders } from "@angular/common/http";
import "rxjs/add/operator/map";
import 'rxjs/add/operator/do'; // debug
import { Observable } from "rxjs";
import { BehaviorSubject } from 'rxjs';
// To inject the dependancies in the service

@Injectable({
  providedIn: 'root'
})
export class RegisteredUsersService {
  /*
  public registeredUsersList: Observable<RegisteredUsers[]>;
  private _registeredUsersList: BehaviorSubject<RegisteredUsers[]>;
  private baseUrl: string;
  private dataStore: {
    registeredUsersList: RegisteredUsers[];
  };
  


  constructor(private http: HttpClient) { }

*/
  // Method to get all employees by calling /api/GetAllEmployees
  /*
  getAll() {
    this.http.get(`${this.baseUrl}GetAllEmployees`)
      .map(response => response.json())
      .subscribe(data => {
        this.dataStore.employeeList = data;
        // Adding newly added Employee in the list
        this._employeeList.next(Object.assign({}, this.dataStore).employeeList);
      }, error => console.log('Could not load employee.'));
  }
  */
}
