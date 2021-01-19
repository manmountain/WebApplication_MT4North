import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-my-pages',
  templateUrl: './my-pages.component.html',
  styleUrls: ['./my-pages.component.css']
})

export class MyPagesComponent {
  isFirstStepModal = true;
  emailList = [];

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
