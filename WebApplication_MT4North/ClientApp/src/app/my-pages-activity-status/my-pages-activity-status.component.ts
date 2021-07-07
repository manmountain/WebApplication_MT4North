import { Component, ElementRef, ViewChild, ViewChildren, QueryList } from '@angular/core';
import { Theme, Activity, ActivityPhase, ActivityStatus } from "../_models";
import { AlertService, ViewService, ProjectService } from "../_services";
import { first } from 'rxjs/operators';
import { Subscription } from 'rxjs';

//import html2canvas from 'html2canvas';

//import { Console } from 'console';

@Component({
  selector: 'app-my-pages-activity-status',
  templateUrl: './my-pages-activity-status.component.html',
  styleUrls: ['./my-pages-activity-status.component.css']
})

export class MyPagesActivityStatusComponent {
  phases = ActivityPhase;
  themes: Theme[] = [];
  activities: Activity[] = [];
  testThemes: Theme[] = [];
  hideExcluded: boolean = false;
  hideFinished: boolean = false;
  isScreenshotting: boolean = false;
  themesSubscription: Subscription;
  activitiesSubscription: Subscription;

  error = '';

  @ViewChildren('themeElement', { read: ElementRef }) themeElements: QueryList<ElementRef>;
  @ViewChildren('activityElement', { read: ElementRef }) activityElements: QueryList<ElementRef>;
  @ViewChild('imTableView', { static: false }) imTableView: ElementRef;
  @ViewChild('canvas', { static: false }) canvas: ElementRef;
  @ViewChild('downloadLink', { static: false }) downloadLink: ElementRef;

  selectedDate = new Date().toISOString().split('T')[0];
  isFullscreen: boolean = false;

  constructor(
    private viewService: ViewService,
    private projectService: ProjectService,
    private alertService: AlertService) {
    this.themesSubscription = this.projectService.themes.subscribe(x => { this.themes = x; console.log('THEMES IN DB:', x); });
    this.projectService.getThemes().pipe(first())
      .subscribe(
        data => {
          console.log('DATA themes: ', data);
        },

        error => {
          console.log('error getting themes: ', error);
          this.error = error;
          this.alertService.error(error);
        });

    this.activitiesSubscription = this.projectService.activities.subscribe(x => { this.activities = x; console.log('ACTIVITIES IN DB:', x); });
    this.projectService.getActivities().pipe(first())
      .subscribe(
        data => {
        },

        error => {
          this.error = error;
          this.alertService.error(error);
        });
  }

  ngOnInit() {
  }

  getPhases() {
    
  }

  ngOnDestroy() {
    this.themesSubscription.unsubscribe();
    this.activitiesSubscription.unsubscribe();
  }

  getProgress(theme: Theme, phase: ActivityPhase): number {
    let activityValBase = 100 / this.activities.filter(x => x.baseactivityinfo.themeid == theme.themeid && x.baseactivityinfo.phase == phase && x.isexcluded == false).length;
    var nrOfStarted = this.activities.filter(x => x.baseactivityinfo.themeid == theme.themeid && x.status != ActivityStatus.NOTSTARTED && x.baseactivityinfo.phase == phase && x.isexcluded == false).length;

    let activityValCustom = 100 / this.activities.filter(x => x.customactivityinfo.themeid == theme.themeid && x.customactivityinfo.phase == phase && x.isexcluded == false).length;
    nrOfStarted += this.activities.filter(x => x.customactivityinfo.themeid == theme.themeid && x.status != ActivityStatus.NOTSTARTED && x.customactivityinfo.phase == phase && x.isexcluded == false).length;

    return (activityValBase+activityValCustom) * nrOfStarted;
  }

  isBaseActivity(activity: Activity) {
    return activity.baseactivityinfoid != null;
  }

  containsOngoingAcitvities(theme: Theme, phase: ActivityPhase): boolean {
    //for (let activity of this.activities) {
    //}
    let ongoingBaseActivities = this.activities.filter(x => x.baseactivityinfo.themeid == theme.themeid && x.baseactivityinfo.phase == phase && x.status == ActivityStatus.ONGOING && x.isexcluded == false);
    let ongoingCustomActivities = this.activities.filter(x => x.customactivityinfo.themeid == theme.themeid && x.customactivityinfo.phase == phase && x.status == ActivityStatus.ONGOING && x.isexcluded == false);

    return ongoingBaseActivities.length > 0 || ongoingCustomActivities.length > 0;
  }

  onHideExcludedChanged(value: boolean) {
    this.hideExcluded = value;
  }

  onHideFinishedChanged(value: boolean) {
    this.hideFinished = value;
  }

  checkStatus(activity: Activity) {
    switch (activity.status) {
      case 0: {
        activity.status = ActivityStatus.NOTSTARTED;
        break;
      }
      case 1: {
        activity.status = ActivityStatus.ONGOING;
        break;
      }
      case 2: {
        activity.status = ActivityStatus.FINISHED;
        break;
      }
    }
  }

  toggleActivityIsExcluded(activity: Activity) {
    activity.isexcluded = !activity.isexcluded;
  }

  hasActivities(theme: Theme, phase: ActivityPhase) {
    console.log('ACTIVITIES: ', this.activities);
    let baseActivities = this.activities.filter(x => x.baseactivityinfo != null && x.baseactivityinfo.themeid == theme.themeid && x.baseactivityinfo.phase == phase && x.isexcluded == false);
    let customActivities = this.activities.filter(x => x.customactivityinfo != null && x.customactivityinfo.themeid == theme.themeid && x.customactivityinfo.phase == phase && x.isexcluded == false);

    return baseActivities.length > 0 || customActivities.length > 0;
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
