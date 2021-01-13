import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule, Routes } from '@angular/router';


//import { ConnectionService } from './connection.service';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { LoginComponent } from './login/login.component';
import { FooterComponent } from './footer/footer.component';
import { ContactUsComponent } from './contact-us/contact-us.component';
import { MyPagesComponent } from './my-pages/my-pages.component';
import { MyPagesMembersComponent } from './my-pages-members/my-pages-members.component';
import { MyPagesProjectSettingsComponent } from './my-pages-project-settings/my-pages-project-settings.component';
import { MyPagesStartComponent } from './my-pages-start/my-pages-start.component';
import { MyPagesProjectComponent } from './my-pages-project/my-pages-project.component';
import { MyPagesActivityStatusComponent } from './my-pages-activity-status/my-pages-activity-status.component';
import { MyPagesActivityComponent } from './my-pages-activity/my-pages-activity.component';
import { MatchesPhasePipe } from './models/matchesPhasePipe'; 
import { EnumToArrayPipe } from './models/enumToArrayPipe';

const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'counter', component: CounterComponent },
  { path: 'fetch-data', component: FetchDataComponent },
  { path: 'login', component: LoginComponent },
  { path: 'footer', component: FooterComponent },
  { path: 'contact-us', component: ContactUsComponent },
  {
    path: 'my-pages',
    component: MyPagesComponent,
    children: [
      {
        path: 'start',
        component: MyPagesStartComponent
      },
      {
        path: 'projects',
        component: MyPagesProjectComponent,
        children: [
          {
            path: 'activity-status',
            component: MyPagesActivityStatusComponent
          },
          {
            path: 'activity',
            component: MyPagesActivityComponent
          },
          {
            path: 'members',
            component: MyPagesMembersComponent
          },
          {
            path: 'settings',
            component: MyPagesProjectSettingsComponent
          }
        ]
      }
    ]
  }

];

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    LoginComponent,
    FooterComponent,
    ContactUsComponent,
    MyPagesComponent,
    MyPagesStartComponent,
    MyPagesProjectComponent,
    MyPagesActivityStatusComponent,
    MyPagesActivityComponent,
    MyPagesMembersComponent,
    MyPagesProjectSettingsComponent,
    MatchesPhasePipe,
    EnumToArrayPipe
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
   // ConnectionServiceModule,
    RouterModule.forRoot(routes)
    
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
export const AppComponents = [] 
