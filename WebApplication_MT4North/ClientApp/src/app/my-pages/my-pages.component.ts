import { Component, ElementRef, ViewChild } from '@angular/core';
import { ViewService } from "../_services";
import { AccountService } from '@app/_services';
import { User } from '../_models';

@Component({
  selector: 'app-my-pages',
  templateUrl: './my-pages.component.html',
  styleUrls: ['./my-pages.component.css']
})

export class MyPagesComponent {
  isFirstStepModal = true;
  emailList = [];
  currentUser: User;

  constructor(
    private viewService: ViewService,
    private accountService: AccountService
  ) {
    this.accountService.currentUser.subscribe(x => { this.currentUser = x; console.log('subscribe user: ', this.currentUser); }, e => console.log(JSON.stringify(e)));
 }

  isFullscreen() {
    return this.viewService.isFullscreen;
  }

  changeToInviteMembers() {
    this.isFirstStepModal = false;
  }

  resetModal() {
    this.isFirstStepModal = true;
    this.emailList = [];
  }

  addMember(emailAdress: string) {
    console.log(emailAdress);
    this.emailList.push(emailAdress);
  }

  removeMember(emailAdress: string) {
    this.emailList.splice(0,1);
  }
}
