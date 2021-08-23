import { Component, ElementRef, ViewChild, ViewChildren, QueryList } from '@angular/core';
import { Theme, Activity, ActivityPhase, ActivityStatus, ActivityInfo, UserProject, User, ProjectRights, ProjectRole, Note } from "../_models";
import { AddActivityModal } from "../_modals";
import { AlertService, ViewService, ProjectService, AccountService } from "../_services";
import { first } from 'rxjs/operators';
import { Subscription } from 'rxjs';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DatePipe } from '@angular/common';

//import html2canvas from 'html2canvas';

//import { Console } from 'console';

@Component({
  selector: 'app-my-pages-activity-status',
  templateUrl: './my-pages-activity-status.component.html',
  styleUrls: ['./my-pages-activity-status.component.css'],
})

export class MyPagesActivityStatusComponent {
  phases = ActivityPhase;
  themes: Theme[] = [];
  noteForm: FormGroup;
  activityInfoForm: FormGroup;
  editActivityInfoForm: FormGroup;
  editActivityDatesForm: FormGroup;
  activities: Activity[] = [];
  testThemes: Theme[] = [];
  hideExcluded: boolean = false;
  hideFinished: boolean = false;
  isScreenshotting: boolean = false;
  themesSubscription: Subscription;
  activitiesSubscription: Subscription;
  userProjectsSubscription: Subscription;
  isDataLoaded = false;
  userProjects: UserProject[];
  hasEditRights = false;
  hasAdminRights = false;
  isAdmin = false;
  currentUser: User;
  currentNote: Note;
  currentActivity: Activity;
  currentActivityInfo = new ActivityInfo();
  currentTheme: Theme;
  currentPhase: ActivityPhase;
  error = '';
  submittedActivity = false;

  @ViewChildren('themeElement', { read: ElementRef }) themeElements: QueryList<ElementRef>;
  @ViewChildren('activityElement', { read: ElementRef }) activityElements: QueryList<ElementRef>;
  @ViewChild('imTableView', { static: false }) imTableView: ElementRef;
  @ViewChild('canvas', { static: false }) canvas: ElementRef;
  @ViewChild('downloadLink', { static: false }) downloadLink: ElementRef;
  @ViewChild('closeAddActivityModal', { static: false }) closeAddActivityModal; 
  @ViewChild('closeEditActivityModal', { static: false }) closeEditActivityModal;
  @ViewChild('closeEditActivityDatesModal', { static: false }) closeEditActivityDatesModal; 
  @ViewChild('openEditActivityModalButton', { static: false }) openEditActivityModalButton;
  @ViewChild('openEditActivityDatesModalButton', { static: false }) openEditActivityDatesModalButton;
  

  selectedDate = new Date().toISOString().split('T')[0];
  isFullscreen: boolean = false;

  constructor(
    private viewService: ViewService,
    private projectService: ProjectService,
    private alertService: AlertService,
    private formBuilder: FormBuilder,
    private accountService: AccountService,
    private datePipe: DatePipe) {
    this.themesSubscription = this.projectService.themes.subscribe(x => { this.themes = x;});
    this.projectService.getThemes().pipe(first())
      .subscribe(
        data => {
          this.activitiesSubscription = this.projectService.activities.subscribe(x => { this.activities = x; });
          this.projectService.getActivities().pipe(first())
            .subscribe(
              data => {
                this.isDataLoaded = true;

              },

              error => {
                const err = error.error.message || error.statusText;
                console.log('error getting activities: ', err);
                this.error = err;
                this.alertService.error(err);
              });        },

        error => {
          const err = error.error.message || error.statusText;
          console.log('error getting themes: ', err);
          this.error = err;
          this.alertService.error(err);
        });

    this.userProjectsSubscription = this.projectService.userProjects.subscribe(x => {
      this.userProjects = x;
      this.currentUser = this.accountService.currentUserValue;
      let currentUserProject = this.userProjects.filter(x => x.userid == this.currentUser.id)[0];
      this.hasEditRights = currentUserProject.rights == ProjectRights.READWRITE;
      this.hasAdminRights = currentUserProject.role == ProjectRole.OWNER;

    });

    //this.accountSubscription = this.accountService.currentUser.subscribe(x => { this.currentUser = x; });

  }

  equalsCurrentPhase(a: ActivityPhase, b: ActivityPhase) {
    console.log(a + '==' + b + ':' + a.toString() == b.toString());
    return (a.toString() == b.toString());
  }

  ngOnInit() {
    this.noteForm = this.formBuilder.group({
      noteid: [0, Validators.required],
      activityid: ['', Validators.required],
      userid: [this.currentUser.id, Validators.required],
      timestamp: ['', Validators.required],
      text: ['', Validators.required]
    });

    this.activityInfoForm = this.formBuilder.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      isBaseActivity: [false]
    });

    this.editActivityInfoForm = this.formBuilder.group({
      name: [Validators.required],
      description: [Validators.required],
      phase: [Validators.required],
      theme: [Validators.required],
      isBaseActivity: [false]
    });

    this.editActivityDatesForm = this.formBuilder.group({
      deadline: [],
      started: [],
      finished: []
    });

  }

  ngOnDestroy() {
    this.themesSubscription.unsubscribe();
    this.activitiesSubscription.unsubscribe();
    this.userProjectsSubscription.unsubscribe();
    //this.accountSubscription.unsubscribe();
  }

  sortNotes(notes) {
    return notes.sort((a, b) => {
      return <any>new Date(b.timestamp) - <any>new Date(a.timestamp);
    });
  }

  getProgress(theme: Theme, phase: ActivityPhase): number {
    let nrOfBaseActivities = this.activities.filter(x => x.baseactivityinfo != null).filter(x => x.baseactivityinfo.themeid == theme.themeid && x.baseactivityinfo.phase == phase && x.isexcluded == false).length;
    let nrOfStartedBase = this.activities.filter(x => x.baseactivityinfo != null).filter(x => x.baseactivityinfo.themeid == theme.themeid && x.status != ActivityStatus.NOTSTARTED && x.baseactivityinfo.phase == phase && x.isexcluded == false).length;

    let nrOfCustomActivities = this.activities.filter(x => x.customactivityinfo != null).filter(x => x.customactivityinfo.themeid == theme.themeid && x.customactivityinfo.phase == phase && x.isexcluded == false).length 
    let nrOfStartedCustom = this.activities.filter(x => x.customactivityinfo != null).filter(x => x.customactivityinfo.themeid == theme.themeid && x.status != ActivityStatus.NOTSTARTED && x.customactivityinfo.phase == phase && x.isexcluded == false).length;

    let totalNrOfActivities = nrOfBaseActivities + nrOfCustomActivities
    let activityVal = totalNrOfActivities > 0 ? 100 / totalNrOfActivities : 0;

    return (nrOfStartedBase + nrOfStartedCustom) * activityVal;
  }

  isBaseActivity(activity: Activity) {
    return activity.baseactivityinfoid != null;
  }

  isCurrentTheme(optionTheme: Theme, currentActivity: Activity) {
    if (this.isBaseActivity(currentActivity)) {
      return optionTheme.name == currentActivity.baseactivityinfo.theme.name;
      } else {
      return optionTheme.name == currentActivity.customactivityinfo.theme.name;

    }
  }

  getUserName(userid: string): String {
    let user = this.userProjects.filter(x => x.user.id == userid)[0].user;
    return user.firstname + " " + user.lastname;
  }

  containsOngoingActivities(theme: Theme, phase: ActivityPhase): boolean {

    let ongoingBaseActivities = this.activities.filter(x => x.baseactivityinfo != null).filter(x =>  x.baseactivityinfo.themeid == theme.themeid && x.baseactivityinfo.phase == phase && x.status == ActivityStatus.ONGOING && x.isexcluded == false);
    let ongoingcustomactivities = this.activities.filter(x => x.customactivityinfo != null).filter(x => x.customactivityinfo.themeid == theme.themeid && x.customactivityinfo.phase == phase && x.status == ActivityStatus.ONGOING && x.isexcluded == false);

    return ongoingBaseActivities.length > 0 || ongoingcustomactivities.length > 0;
  }

  onHideExcludedChanged(value: boolean) {
    this.hideExcluded = value;
  }

  onHideFinishedChanged(value: boolean) {
    this.hideFinished = value;
  }

  updateStatus(activity: Activity) {
    switch (activity.status) {
      case ActivityStatus.NOTSTARTED: {
        activity.status = ActivityStatus.ONGOING;
        this.updateActivity(activity);

        break;
      }
      case ActivityStatus.ONGOING: {
        activity.status = ActivityStatus.FINISHED;
        this.updateActivity(activity);

        break;
      }
      case ActivityStatus.FINISHED: {
        activity.status = ActivityStatus.NOTSTARTED;
        this.updateActivity(activity);

        break;
      }
    }
  }

  // convenience getter for easy access to form fields
  get f() { return this.activityInfoForm.controls; }
  get fEdit() { return this.editActivityInfoForm.controls; }
  get fEditDates() { return this.editActivityDatesForm.controls; }

  addActivity() {
    this.submittedActivity = true;

    console.log('adding activity');
    if (this.activityInfoForm.invalid) {
      return;
    }
    this.alertService.clear();

    let activity = new Activity();
    activity.projectid = this.userProjects[0].projectid;

    if (this.activityInfoForm.controls.isBaseActivity.value) {
      activity.baseactivityinfo = new ActivityInfo();
      activity.baseactivityinfoid = 0;
      activity.baseactivityinfo.baseactivityid = 0;
      activity.baseactivityinfo.name = this.activityInfoForm.controls.name.value;
      activity.baseactivityinfo.description = this.activityInfoForm.controls.description.value;
      activity.baseactivityinfo.themeid = this.currentTheme.themeid;
      activity.baseactivityinfo.phase = this.currentPhase;
    } else {
      activity.customactivityinfo = new ActivityInfo();
      activity.customactivityinfoid = 0;
      activity.customactivityinfo.customactivityid = 0;
      activity.customactivityinfo.name = this.activityInfoForm.controls.name.value;
      activity.customactivityinfo.description = this.activityInfoForm.controls.description.value;
      activity.customactivityinfo.themeid = this.currentTheme.themeid;
      activity.customactivityinfo.phase = this.currentPhase = this.currentPhase;
    }

    this.projectService.createActivity(activity)
      .pipe(first())
      .subscribe(
        data => {
          this.clearAddActivityForm();
          this.closeAddActivityModal.nativeElement.click();
          this.alertService.success('Dina ändringar har sparats.', { keepAfterRouteChange: true });
        },
        error => {
          this.clearAddActivityForm();
          this.closeAddActivityModal.nativeElement.click();
          const err = error.error.message || error.statusText;
          this.alertService.error(err);
        });
  }

  updateActivity(activity: Activity) {
    this.alertService.clear();
    this.projectService.updateActivity(activity.activityid, activity)
      .pipe(first())
      .subscribe(
        data => {
          // console.log('activity updated: ', data);
          this.alertService.success('Dina ändringar har sparats.', { keepAfterRouteChange: true });
        },
        error => {
          if (error.status == 403) {
              this.alertService.error('Otillåtet. Du måste ha läs och skriv rättigheter för att ta ändra aktiviteten');
          } else if (error.status == 404) {
            this.alertService.error('Aktiviteten hittades inte. Försök igen senare');
          } else {
            this.alertService.error('Okänt fel. Kontakta support eller försök igen senare. Felkod: ', error.status);
          }
        });
  }

  clearAddActivityForm() {
    this.activityInfoForm.controls.name.setValue('');
    this.activityInfoForm.controls.description.setValue('');
  }

  editActivity() {
    // Fixa så att det inte blir null ifall ingen ändring görs i ett fält.
    if (this.isBaseActivity(this.currentActivity)) {
      this.currentActivity.baseactivityinfo.name = this.editActivityInfoForm.controls.name.value;
      this.currentActivity.baseactivityinfo.description = this.editActivityInfoForm.controls.description.value;
      this.currentActivity.baseactivityinfo.theme = this.editActivityInfoForm.controls.theme.value;
      this.currentActivity.baseactivityinfo.themeid = this.currentActivity.baseactivityinfo.theme.themeid
      this.currentActivity.baseactivityinfo.phase = this.editActivityInfoForm.controls.phase.value;
    } else {
      this.currentActivity.customactivityinfo.name = this.editActivityInfoForm.controls.name.value;
      this.currentActivity.customactivityinfo.description = this.editActivityInfoForm.controls.description.value;
      this.currentActivity.customactivityinfo.theme = this.editActivityInfoForm.controls.theme.value;
      this.currentActivity.customactivityinfo.themeid = this.currentActivity.customactivityinfo.theme.themeid
      this.currentActivity.customactivityinfo.phase = this.editActivityInfoForm.controls.phase.value;
    }
    this.updateActivity(this.currentActivity)
  }

  editActivityDates() {
    let edits = 0;
    if (this.editActivityDatesForm.controls.deadline.value && this.editActivityDatesForm.controls.deadline.value.trim().length > 0) {
      this.currentActivity.deadlinedate = this.editActivityDatesForm.controls.deadline.value;
      edits++;
    }
    if (this.editActivityDatesForm.controls.started.value && this.editActivityDatesForm.controls.started.value.trim().length > 0) {
      this.currentActivity.startdate = this.editActivityDatesForm.controls.started.value;
      edits++;
    }
    if (this.editActivityDatesForm.controls.finished.value && this.editActivityDatesForm.controls.finished.value.trim().length > 0) {
      this.currentActivity.finishdate = this.editActivityDatesForm.controls.finished.value;
      edits++;
    }
    if (edits > 0) {
      // console.log(edits + 'st ändringar sparas');
      this.updateActivity(this.currentActivity);
    } /*else {
      console.log('Inga ändringar sparas');
      //this.alertService.error('Inga ändringar sparades');
    }*/
    this.closeEditActivityDatesModal.nativeElement.click();
  }

  deleteActivity(activityid: string, activityinfoid: string, isBaseActivity: boolean) {
    this.projectService.deleteActivity(parseInt(activityid), isBaseActivity)
      .pipe(first())
      .subscribe(
        activity => {
          this.alertService.success('Aktiviteten har tagits bort', { keepAfterRouteChange: true });
        },
        error => {
          if (error.status == 403) {
            if (isBaseActivity) {
              this.alertService.error('Otillåtet. Du måste ha Administrations rättigheter för att ta bort en bas aktivitet');
            } else {
              this.alertService.error('Otillåtet. Du måste ha läs och skriv rättigheter för att ta bort aktiviteten');
            }
          } else if (error.status == 404) {
            this.alertService.error('Aktiviteten hittades inte. Försök igen senare');
          } else {
            this.alertService.error('Okänt fel. Kontakta support eller försök igen senare. Felkod: ' + error.status);
          }
        }
      );
  }

  addNote(activity: Activity) {

    this.noteForm.controls.timestamp.setValue(new Date().toJSON());
    this.noteForm.controls.activityid.setValue(activity.activityid);

    if (this.noteForm.invalid) {
      return;
    }

    this.alertService.clear();

    this.projectService.addNote(activity.activityid, this.noteForm.value)
      .pipe(first())
      .subscribe(
        data => {
          this.alertService.success('Anteckningen har lagts till', { keepAfterRouteChange: true });
          this.noteForm.controls.text.setValue('');
        },
        error => {
          const err = error.error.message || error.statusText;
          this.alertService.error(err);
        });
  }

  editNote(activity: Activity, note: Note) {

  }

  removeNote() {
    this.alertService.clear();

    this.projectService.removeNote(this.currentActivity.activityid, this.currentNote.noteid)
      .pipe(first())
      .subscribe(
        data => {
          this.alertService.success('Anteckningen har tagits bort', { keepAfterRouteChange: true });
        },
        error => {
          const err = error.error.message || error.statusText;
          this.alertService.error(err);
        });
  }

  toggleActivityIsExcluded(activity: Activity) {
    activity.isexcluded = !activity.isexcluded;

    this.updateActivity(activity);
  }

  hasActivities(theme: Theme, phase: ActivityPhase) {
    let baseActivities = this.activities.filter(x => x.baseactivityinfo != null).filter(x => x.baseactivityinfo.themeid == theme.themeid && x.baseactivityinfo.phase == phase && x.isexcluded == false);
    let customActivities = this.activities.filter(x => x.customactivityinfo != null).filter(x => x.customactivityinfo.themeid == theme.themeid && x.customactivityinfo.phase == phase && x.isexcluded == false);

    return baseActivities.length > 0 || customActivities.length > 0;
    //return baseActivities.length > 0;

  }

  setCurrentActivityDates(activity: Activity) {
    this.currentActivity = activity;
    this.currentActivityInfo = (activity.baseactivityinfoid != null) ? activity.baseactivityinfo : activity.customactivityinfo;
    this.openEditActivityDatesModalButton.nativeElement.click();
  }

  setCurrentNote(note: Note, activity: Activity) {
    this.currentNote = note;
    this.currentActivity = activity;
  }

  setCurrentActivityInfo(activity: Activity) {
    this.currentActivity = activity;
    this.currentActivityInfo = (activity.baseactivityinfoid != null) ? activity.baseactivityinfo : activity.customactivityinfo;
    this.openEditActivityModalButton.nativeElement.click();
  }

  setCurrentThemeAndPhase(theme: Theme, phase: ActivityPhase) {
    this.currentTheme = theme;
    this.currentPhase = phase;
  }

  expandAll() {
    this.expandThemes()
    this.expandActivities();
  }

  expandThemes() {
    this.themeElements.toArray().forEach(val => { if (val.nativeElement.getAttribute('aria-expanded') === "false") { val.nativeElement.click() } });
  }

  expandActivities() {
    console.log("nr of activityElements: ", this.activityElements.toArray().length);
    this.activityElements.toArray().forEach(val => { if (val.nativeElement.getAttribute('aria-expanded') === "false") { val.nativeElement.click() } });
  }

  collapseAll() {
    this.collapseThemes();
    this.collapseActivities();
  }

  collapseThemes() {
    this.themeElements.toArray().forEach(val => { if (val.nativeElement.getAttribute('aria-expanded') === "true") { val.nativeElement.click() } });
  }

  collapseActivities() {
    this.activityElements.toArray().forEach(val => { if (val.nativeElement.getAttribute('aria-expanded') === "true") { val.nativeElement.click() } });
  }

  getMaxDate() {
    return new Date().toISOString().split('T')[0];
  }

  toggleExpandView() {
    this.isFullscreen = !this.isFullscreen;
    this.viewService.isFullscreen = this.isFullscreen;
    this.isFullscreen ? this.openfullscreen() : this.closefullscreen();
  }

  openfullscreen() {
    // Trigger fullscreen
    const docElmWithBrowsersFullScreenFunctions = document.documentElement as HTMLElement & {
      mozRequestFullScreen(): Promise<void>;
      webkitRequestFullscreen(): Promise<void>;
      msRequestFullscreen(): Promise<void>;
    };

    if (docElmWithBrowsersFullScreenFunctions.requestFullscreen) {
      docElmWithBrowsersFullScreenFunctions.requestFullscreen();
    } else if (docElmWithBrowsersFullScreenFunctions.mozRequestFullScreen) { /* Firefox */
      docElmWithBrowsersFullScreenFunctions.mozRequestFullScreen();
    } else if (docElmWithBrowsersFullScreenFunctions.webkitRequestFullscreen) { /* Chrome, Safari and Opera */
      docElmWithBrowsersFullScreenFunctions.webkitRequestFullscreen();
    } else if (docElmWithBrowsersFullScreenFunctions.msRequestFullscreen) { /* IE/Edge */
      docElmWithBrowsersFullScreenFunctions.msRequestFullscreen();
    }
  }

  closefullscreen() {
    const docWithBrowsersExitFunctions = document as Document & {
      mozCancelFullScreen(): Promise<void>;
      webkitExitFullscreen(): Promise<void>;
      msExitFullscreen(): Promise<void>;
    };
    if (docWithBrowsersExitFunctions.exitFullscreen) {
      docWithBrowsersExitFunctions.exitFullscreen();
    } else if (docWithBrowsersExitFunctions.mozCancelFullScreen) { /* Firefox */
      docWithBrowsersExitFunctions.mozCancelFullScreen();
    } else if (docWithBrowsersExitFunctions.webkitExitFullscreen) { /* Chrome, Safari and Opera */
      docWithBrowsersExitFunctions.webkitExitFullscreen();
    } else if (docWithBrowsersExitFunctions.msExitFullscreen) { /* IE/Edge */
      docWithBrowsersExitFunctions.msExitFullscreen();
    }
  }

  makeScreenshot() {
    this.isScreenshotting = true;

    this.collapseAll();

    /*
    html2canvas(this.imTableView.nativeElement).then(canvas => {
      //this.canvas.nativeElement.src = canvas.toDataURL();
      this.downloadLink.nativeElement.href = canvas.toDataURL('image/png');
      this.downloadLink.nativeElement.download = 'project-name-' + this.selectedDate +'.png';
      this.downloadLink.nativeElement.click();
      this.isScreenshotting = false;
      console.log('screenshot created');
    });
    */
    /*this.imTableView.nativeElement.removeClass += " animate";*/


  }
}
