import { Component, ElementRef, ViewChild, ViewChildren, QueryList } from '@angular/core';
import { Theme } from "../models/theme.model";
import { Activity } from "../models/activity.model";
import { Phase } from "../models/common";
import { Status } from "../models/common";
import { ViewService } from "../view.service";
import html2canvas from 'html2canvas';

//import { Console } from 'console';

@Component({
  selector: 'app-my-pages-activity-status',
  templateUrl: './my-pages-activity-status.component.html',
  styleUrls: ['./my-pages-activity-status.component.css']
})

export class MyPagesActivityStatusComponent {
  phases = Phase;
  themes: Theme[] = [];
  isScreenshotting: boolean = false;
  @ViewChildren('themeElement', { read: ElementRef }) themeElements: QueryList<ElementRef>;
  @ViewChildren('activityElement', { read: ElementRef }) activityElements: QueryList<ElementRef>;
  @ViewChild('imTableView', { static: false }) imTableView: ElementRef;
  @ViewChild('canvas', { static: false }) canvas: ElementRef;
  @ViewChild('downloadLink', { static: false }) downloadLink: ElementRef;

  selectedDate = new Date().toISOString().split('T')[0];
  isFullscreen: boolean = false;

  constructor(private viewService: ViewService) {
  }

  ngOnInit() {
    //console.log("initiating");
    let technicalDevelopmentTheme = new Theme("Teknikutveckling", "Beskrivning");
    let activity1 = new Activity("Observera grundläggande principer", "Utveckla ett grundläggande koncept som svarar mot behov.", Phase.CONCEPTUALIZATION, false);
    let activity2 = new Activity("Genomför kravanalys", "Genomför en kravanalys för konceptet. Finns standarder att förhålla sig till?", Phase.CONCEPTUALIZATION, true);
    let activity3 = new Activity("Utveckla prototyp baserat på koncept", "En första prototyp ska utvecklas som kan användas både som proof-of-concept och för att genomföra nödvändiga användartester i detta skede.", Phase.PROOFOFCONCEPT, false);
    technicalDevelopmentTheme.addActivity(activity1);
    technicalDevelopmentTheme.addActivity(activity2);
    technicalDevelopmentTheme.addActivity(activity3);

    this.themes.push(technicalDevelopmentTheme);

    let clinicalDevelopmentTheme = new Theme("Klinisk utveckling", "Beskrivning");
    let activity4 = new Activity("Kontakta klinisk personal", "Kontakta klinisk personal inom relevant område för att bekräfta att produktidén svarar mot ett faktiskt, upplevt behov inom vården, alternativt innebär en för dem relevant förbättring av en existerande lösning. Undersök möjlighet till samarbeten för framtida klinisk utvärdering av prototyp/produkt.", Phase.CONCEPTUALIZATION, false);
    let activity5 = new Activity("Genomför klinisk riskanalys", "Genomför en riskanalys i enlighet med relevant standard, tillsammans med personer med nödvändig klinisk kompetens.", Phase.PROOFOFCONCEPT, false);
    let activity6 = new Activity("Genomför kliniska tester i laboratoriemiljö", "Testa att prototypen uppfyller de tänkta kliniska grundfunktionerna i relevant laboratoriemiljö.", Phase.PROOFOFCONCEPT, false);
    clinicalDevelopmentTheme.addActivity(activity4);
    clinicalDevelopmentTheme.addActivity(activity5);
    clinicalDevelopmentTheme.addActivity(activity6);

    this.themes.push(clinicalDevelopmentTheme);

  }

  getProgress(theme: Theme, phase: Phase): number {
    let activityVal = 100 / theme.activities.filter(x => x.phase == phase && x.isExcluded == false).length;
    let nrOfStarted = theme.activities.filter(x => x.status != Status.NOTSTARTED && x.phase == phase && x.isExcluded == false).length;

    return activityVal * nrOfStarted;
  }

  containsOngoingAcitvities(theme: Theme, phase: Phase): boolean {
    return theme.activities.filter(x => x.phase == phase && x.status == Status.ONGOING && x.isExcluded == false).length > 0;
  }

  checkStatus(activity: Activity) {
    switch (activity.status) {
      case "checked": {
        activity.status = Status.NOTSTARTED;
        break;
      }
      case "unchecked": {
        activity.status = Status.ONGOING;
        break;
      }
      case "crossed": {
        activity.status = Status.FINISHED;
        break;
      }
    }
  }

  toggleActivityIsExcluded(activity: Activity) {
    activity.isExcluded = !activity.isExcluded;
  }

  hasActivities(theme: Theme, phase: Phase) {
    return theme.activities.filter(x => x.phase == phase && x.isExcluded == false).length > 0;
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

    html2canvas(this.imTableView.nativeElement).then(canvas => {
      /*this.canvas.nativeElement.src = canvas.toDataURL();*/
      this.downloadLink.nativeElement.href = canvas.toDataURL('image/png');
      this.downloadLink.nativeElement.download = 'project-name-' + this.selectedDate +'.png';
      this.downloadLink.nativeElement.click();
      this.isScreenshotting = false;
      console.log('screenshot created');
    });

    /*this.imTableView.nativeElement.removeClass += " animate";*/


  }
}
