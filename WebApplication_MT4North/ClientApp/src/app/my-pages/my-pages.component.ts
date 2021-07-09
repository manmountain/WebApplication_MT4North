import { Component, OnDestroy, ElementRef, ViewChild } from '@angular/core';
import { ViewService } from "../_services";
import { AlertService, AccountService, ProjectService } from '@app/_services';
import { User, Project, UserInvitation, ProjectRole, ProjectRights } from '../_models';
import { first } from 'rxjs/operators';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-my-pages',
  templateUrl: './my-pages.component.html',
  styleUrls: ['./my-pages.component.css']
})

export class MyPagesComponent implements OnDestroy {
  isFirstStepModal = true;
  permissions = ProjectRights;
  roles = ProjectRole;
  userInvitations: UserInvitation[] = [];
  currentUser: User;
  projects: Project[];
  projectForm: FormGroup;
  newProjectId: string;
  invitationForm: FormGroup;
  loading = false;
  submitted = false;
  error = '';
  accountSubscription: Subscription;
  projectsSubscription: Subscription;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private viewService: ViewService,
    private alertService: AlertService,
    private accountService: AccountService,
    private projectService: ProjectService,
    private formBuilder: FormBuilder
  ) {
    this.accountSubscription = this.accountService.currentUser.subscribe(x => { this.currentUser = x; console.log('subscribe user: ', this.currentUser); }, e => console.log(JSON.stringify(e)));
    this.projectsSubscription = this.projectService.projects.subscribe(x => this.projects = x);
    this.projectService.getProjects()
      .pipe(first())
      .subscribe(
        data => { console.log('reloaded nav menu');},

        error => {
          this.error = error;
          this.alertService.error(error);
        });
    //console.log("projects in my-pages: ", this.projects[0].name);

  }

  ngOnInit() {
    this.projectForm = this.formBuilder.group({
      name: ['', Validators.required],
      description: ['']
    });
    this.invitationForm = this.formBuilder.group({
      email: ['', Validators.required],
      role: [this.roles.PARTICIPANT],
      permissions: [this.permissions.READ]
    });
  }

  ngOnDestroy() {
    console.log('unsubscribing in nav-menu');
    this.accountSubscription.unsubscribe();
    this.projectsSubscription.unsubscribe();
  }

  selectProject(projectid: string) {
    console.log('project id: ', projectid);
    this.projectService.selectProject(projectid)
      .pipe(first())
      .subscribe(
        data => {
          this.projectService.getUserProjects(projectid)
            .pipe(first())
            .subscribe(
              data => {

                this.projectService.getThemes().pipe(first())
                  .subscribe(
                    data => {

                      this.projectService.getActivities().pipe(first())
                        .subscribe(
                          data => {
                            //this.router.navigate(["/projects/" + projectid + "/activity-status"]);
                            this.router.navigate(['projects', projectid, 'activity-status'], { relativeTo: this.route });


                            //"./projects/{{project?.projectid}}/activity-status"
                          },

                          error => {
                            this.error = error;
                            this.alertService.error(error);
                          });
                    },

                    error => {
                      this.error = error;
                      this.alertService.error(error);
                    });



              },

              error => {
                this.error = error;
                this.alertService.error(error);
              });
        },

        error => {
          this.error = error;
          this.alertService.error(error);
        });




  }

  // convenience getter for easy access to form fields
  get f() { return this.projectForm.controls; }
  get fi() { return this.invitationForm.controls; }


  isFullscreen() {
    return this.viewService.isFullscreen;
  }

  createProject() {
    console.log('form submitted');
    this.submitted = true;

    // reset alerts on submit
    this.alertService.clear();

    // stop here if form is invalid
    if (this.projectForm.invalid) {
      return;
    }
    console.log('form value: ', this.projectForm.value);

    this.loading = true;
    this.projectService.createProject(this.f.name.value, this.f.description.value)
      .pipe(first())
      .subscribe(
        data => {
          console.log('returned from create proj: ', data);
          this.newProjectId = data.projectid;
          this.alertService.success('Projektet har skapats', { keepAfterRouteChange: true });
          this.loading = false;
        },
        error => {
          this.alertService.error(error);
          this.loading = false;
        });
    this.isFirstStepModal = false;
  }

  inviteMembers() {
    // reset alerts on submit
    this.alertService.clear();

    // stop here if form is invalid
    //if (this.projectForm.invalid) {
    //  return;
    //}
    //console.log('form value: ', this.projectForm.value);

    this.loading = true;
    for (var userInvitation of this.userInvitations) {
      console.log('user permissions: ', userInvitation.permissions);
      //let permissions = userInvitation.permissions == "Kan endast läsa" ? "R" : "RW";
      this.projectService.inviteMember(this.newProjectId, userInvitation.email, userInvitation.role, userInvitation.permissions)
        .pipe(first())
        .subscribe(
          data => {
            console.log('user invited from my pages');
            this.alertService.success('Inbjudan har skickats', { keepAfterRouteChange: true });
            this.loading = false;
          },
          error => {
            console.log('user NOT invited from my pages. error: ', error);

            this.alertService.error(error);
            this.loading = false;
          });
      this.isFirstStepModal = false;

    }
  }

  resetModal() {
    this.isFirstStepModal = true;
    this.userInvitations = [];
  }

  addMember(email: string, role: ProjectRole, permissions: ProjectRights) {
    this.userInvitations.push(new UserInvitation(this.userInvitations.length + 1, email, role, permissions));
    console.log('invitations: ', this.userInvitations);
  }

  removeMember(email: string) {
    let idToRemove = this.userInvitations.filter(x => x.email == email)[0].id-1;
    this.userInvitations.splice(idToRemove, 1);
  }
}
