// my-bootstrap-modal.component.ts
import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-add-activity-modal',
  templateUrl: './addActivityModal.component.html'
  //styleUrls: ['./addActivityModal.component.css']
})
export class AddActivityModal implements OnInit {

  @Input() fromParent;

  constructor(  ) { }

  ngOnInit() {
    console.log(this.fromParent);
    /* Output:
     {prop1: "Some Data", prop2: "From Parent Component", prop3: "This Can be anything"}
    */
  }

  closeModal(sendData) {
    //this.activeModal.close(sendData);
  }

}
