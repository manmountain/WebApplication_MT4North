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
  isDataLoaded = false;

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
                console.log('error getting activities: ', error);
                this.error = error;
                this.alertService.error(error);
              });        },

        error => {
          console.log('error getting themes: ', error);
          this.error = error;
          this.alertService.error(error);
        });


  }

  ngOnInit() {
  }

  ngOnDestroy() {
    this.themesSubscription.unsubscribe();
    this.activitiesSubscription.unsubscribe();
  }

  getProgress(theme: Theme, phase: ActivityPhase): number {
    let nrOfBaseActivities = this.activities.filter(x => x.baseactivityinfo != null).filter(x => x.baseactivityinfo.themeid == theme.themeid && x.baseactivityinfo.phase == phase && x.isexcluded == false).length;
    let activityValBase = nrOfBaseActivities > 0 ? 100 / nrOfBaseActivities : 0;
    var nrOfStarted = this.activities.filter(x => x.baseactivityinfo != null).filter(x => x.baseactivityinfo.themeid == theme.themeid && x.status != ActivityStatus.NOTSTARTED && x.baseactivityinfo.phase == phase && x.isexcluded == false).length;

    let nrOfCustomActivities = this.activities.filter(x => x.customactivityinfo != null).filter(x => x.customactivityinfo.themeid == theme.themeid && x.customactivityinfo.phase == phase && x.isexcluded == false).length 
    let activityValCustom = nrOfCustomActivities > 0 ? 100 / nrOfCustomActivities : 0;
    nrOfStarted += this.activities.filter(x => x.customactivityinfo != null).filter(x => x.customactivityinfo.themeid == theme.themeid && x.status != ActivityStatus.NOTSTARTED && x.customactivityinfo.phase == phase && x.isexcluded == false).length;

    return (activityValBase + activityValCustom) * nrOfStarted;
  }

  isBaseActivity(activity: Activity) {
    return activity.baseactivityinfoid != null;
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

  updateActivity(activity: Activity) {
    this.projectService.updateActivity(activity.activityid, activity)
      .pipe(first())
      .subscribe(
        data => {
          console.log('activity updated: ', data);
          this.alertService.success('Aktiviteten har uppdaterats', { keepAfterRouteChange: true });
        },
        error => {
          console.log('activity NOT updated. error: ', error);

          this.alertService.error(error);
        });
  }

  toggleActivityIsExcluded(activity: Activity) {
    activity.isexcluded = !activity.isexcluded;
  }

  hasActivities(theme: Theme, phase: ActivityPhase) {
    let baseActivities = this.activities.filter(x => x.baseactivityinfo != null).filter(x => x.baseactivityinfo.themeid == theme.themeid && x.baseactivityinfo.phase == phase && x.isexcluded == false);
    let customActivities = this.activities.filter(x => x.customactivityinfo != null).filter(x => x.customactivityinfo.themeid == theme.themeid && x.customactivityinfo.phase == phase && x.isexcluded == false);

    return baseActivities.length > 0 || customActivities.length > 0;
    //return baseActivities.length > 0;

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
