//import { Component } from '@angular/core';

//@Component({
//  selector: 'app-home',
//  templateUrl: './home.component.html',
//  styleUrls: ['./home.component.css'],
//})
//export class HomeComponent {
//}

import { Component } from '@angular/core';
import { first } from 'rxjs/operators';

import { User } from '../_models';
import { UserService, AuthenticationService } from '../_services';

@Component({ templateUrl: 'home.component.html' })
export class HomeComponent {
    loading = false;
    users: User[];

    constructor(private userService: UserService) { }

    ngOnInit() {
        this.loading = true;
        //this.userService.getAll().pipe(first()).subscribe(users => {
        //    this.loading = false;
        //    this.users = users;
        //});
    }
}
