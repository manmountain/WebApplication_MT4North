import { Component, OnDestroy } from '@angular/core';
import { ProjectService, AccountService, AlertService } from '@app/_services';
import { User, UserProject, UserInvitation, ProjectRights, ProjectRole } from '../_models';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';



@Component({
  selector: 'app-my-pages-members-component',
  templateUrl: './my-pages-members.component.html',
  styleUrls: ['./my-pages-members.component.css']
})
export class MyPagesMembersComponent implements OnDestroy {
  userProjects: UserProject[];
  userInvitations: UserInvitation[] = [];
  permissions = ProjectRights;
  roles = ProjectRole;
  invitationForm: FormGroup;
  currentUser: User;
  currentUserProject: UserProject;
  userProjectsSubscription: Subscription;
  accountSubscription: Subscription;
  hasRights = false;

  constructor(
    private projectService: ProjectService,
    private alertService: AlertService,
    private accountService: AccountService,
    private formBuilder: FormBuilder) {

    this.accountService.getCurrentUser();
    this.accountSubscription = this.accountService.currentUser.subscribe(x => { this.currentUser = x; });
    this.userProjectsSubscription = this.projectService.userProjects.subscribe(x => {
      this.userProjects = x;
      this.currentUserProject = this.userProjects.filter(x => x.userid == this.currentUser.id)[0];
      this.hasRights = this.currentUserProject.rights == ProjectRights.READWRITE && this.currentUserProject.role == ProjectRole.OWNER;
    });
  }

  ngOnInit() {
    this.invitationForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      role: [this.roles.PARTICIPANT],
      permissions: [this.permissions.READ]
    });
  }

  ngOnDestroy() {
    this.userProjectsSubscription.unsubscribe();
    this.accountSubscription.unsubscribe();

  }

  updateProjectMember(userProject: UserProject) {
    console.log('updated project member data: ', userProject);
    userProject.isEditable = false;
    this.projectService.updateProjectMember(userProject.userprojectid, userProject)
      .pipe(first())
      .subscribe(
        data => {
          console.log('DATA: ', data);
          this.alertService.success('Projektmedlemmen har uppdaterats', { keepAfterRouteChange: true });
        },
        error => {
          this.alertService.error(error);
        });
  }

  inviteMembers() {
    // reset alerts on submit
    this.alertService.clear();

    // stop here if form is invalid
    //if (this.projectForm.invalid) {
    //  return;
    //}
    //console.log('form value: ', this.projectForm.value);

    for (var userInvitation of this.userInvitations) {
      console.log('user permissions: ', userInvitation.permissions);
      //let permissions = userInvitation.permissions == 0 ? 0 : 2;
      this.projectService.inviteMember(this.userProjects[0].projectid, userInvitation.email, userInvitation.role, userInvitation.permissions)
        .pipe(first())
        .subscribe(
          data => {
            console.log('user invited from my pages');
            this.alertService.success('Inbjudan har skickats', { keepAfterRouteChange: true });
            this.resetInvitations();
          },
          error => {
            this.resetInvitations();
            this.alertService.error(error);
          });

    }
  }

  addMember(email: string, role: ProjectRole, permissions: ProjectRights) {
    this.userInvitations.push(new UserInvitation(this.userInvitations.length + 1, email, role, permissions));
    console.log('invitations: ', this.userInvitations);
  }

  removeMember(email: string) {
    let idToRemove = this.userInvitations.filter(x => x.email == email)[0].id-1;
    this.userInvitations.splice(idToRemove, 1);
  }

  resetInvitations() {
    console.log('list emptied');
    this.userInvitations.splice(0, this.userInvitations.length);
  }

  get fi() {
    return this.invitationForm.controls;
  }

}
