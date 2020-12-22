import { Component } from '@angular/core';
import { Theme } from "../models/theme.model";
import { Activity } from "../models/activity.model";
import { Phase } from "../models/common";
import { Status } from "../models/common";
/*import { Console } from 'console';*/

@Component({
  selector: 'app-my-pages-activity-status',
  templateUrl: './my-pages-activity-status.component.html',
  styleUrls: ['./my-pages-activity-status.component.css']
})

export class MyPagesActivityStatusComponent {
  phases = Phase;
  themes: Theme[] = [];

  constructor() {
  }

  ngOnInit() {
    let technicalDevelopmentTheme = new Theme("Teknikutveckling", "Beskrivning");
    let activity1 = new Activity("Observera grundläggande principer", "Utveckla ett grundläggande koncept som svarar mot behov.", Phase.CONCEPTUALIZATION);
    let activity2 = new Activity("Genomför kravanalys", "Genomför en kravanalys för konceptet. Finns standarder att förhålla sig till?", Phase.CONCEPTUALIZATION);
    let activity3 = new Activity("Utveckla prototyp baserat på koncept", "En första prototyp ska utvecklas som kan användas både som proof-of-concept och för att genomföra nödvändiga användartester i detta skede.", Phase.PROOFOFCONCEPT);
    technicalDevelopmentTheme.addActivity(activity1);
    technicalDevelopmentTheme.addActivity(activity2);
    technicalDevelopmentTheme.addActivity(activity3);

    this.themes.push(technicalDevelopmentTheme);

    let clinicalDevelopmentTheme = new Theme("Klinisk utveckling", "Beskrivning");
    let activity4 = new Activity("Kontakta klinisk personal", "Kontakta klinisk personal inom relevant område för att bekräfta att produktidén svarar mot ett faktiskt, upplevt behov inom vården, alternativt innebär en för dem relevant förbättring av en existerande lösning. Undersök möjlighet till samarbeten för framtida klinisk utvärdering av prototyp/produkt.", Phase.CONCEPTUALIZATION);
    let activity5 = new Activity("Genomför klinisk riskanalys", "Genomför en riskanalys i enlighet med relevant standard, tillsammans med personer med nödvändig klinisk kompetens.", Phase.PROOFOFCONCEPT);
    let activity6 = new Activity("Genomför kliniska tester i laboratoriemiljö", "Testa att prototypen uppfyller de tänkta kliniska grundfunktionerna i relevant laboratoriemiljö.", Phase.PROOFOFCONCEPT);
    clinicalDevelopmentTheme.addActivity(activity4);
    clinicalDevelopmentTheme.addActivity(activity5);
    clinicalDevelopmentTheme.addActivity(activity6);

    this.themes.push(clinicalDevelopmentTheme);

  }

  getProgress(theme: Theme, phase: Phase): number {
    let activityVal = 100 / theme.activities.filter(x => x.phase == phase).length;
    let nrOfStarted = theme.activities.filter(x => x.status != Status.NOTSTARTED && x.phase == phase).length;

    return activityVal * nrOfStarted;
  }

  containsOngoingAcitvities(theme: Theme, phase: Phase): boolean {
    return theme.activities.filter(x => x.phase == phase && x.status == Status.ONGOING).length > 0;
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
}
