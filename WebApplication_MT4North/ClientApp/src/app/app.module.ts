import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule, Routes } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MarkdownModule } from 'ngx-markdown';

//import { ConnectionService } from './connection.service';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { AlertComponent } from './_components';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { LoginComponent } from './login';
import { FooterComponent } from './footer/footer.component';
import { ContactUsComponent } from './contact-us/contact-us.component';
import { HelpComponent } from './help/help.component';
import { MyPagesComponent } from './my-pages/my-pages.component';
import { MyPagesMembersComponent } from './my-pages-members/my-pages-members.component';
import { MyPagesProjectSettingsComponent } from './my-pages-project-settings/my-pages-project-settings.component';
import { MyPagesStartComponent } from './my-pages-start/my-pages-start.component';
import { MyPagesProjectComponent } from './my-pages-project/my-pages-project.component';
import { MyPagesActivityStatusComponent } from './my-pages-activity-status/my-pages-activity-status.component';
import { MyPagesActivityComponent } from './my-pages-activity/my-pages-activity.component';
import { MyPagesEditAccountComponent } from './my-pages-edit-account';
import { MyPagesDeleteAccountComponent } from './my-pages-delete-account';
import { AdministrateBaseActivities } from './administrate-base-activities';
import { AdministrateBaseActivitiesEdit } from './administrate-base-activities-edit';
import { AdministrateBaseActivitiesNew } from './administrate-base-activities-new';
import { AdministrateThemes } from './administrate-themes';
import { AdministrateThemesEdit } from './administrate-themes-edit';
import { AdministrateThemesNew } from './administrate-themes-new';
import { AdministrateUsers } from './administrate-users';
import { MatchesPhaseAndThemePipe, EnumToArrayPipe, NumberToPhasePipe, NumberToRolePipe, NumberToRightsPipe, NumberToStatusPipe, JsonToDatePipe } from './_models';
import { AddActivityModal } from './_modals';
import { JwtInterceptor, ErrorInterceptor, AuthGuard } from './_helpers';
import { RegisterComponent } from './register/register.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'counter', component: CounterComponent },
  { path: 'help', component: HelpComponent },
  { path: 'fetch-data', component: FetchDataComponent },
  { path: 'login', component: LoginComponent },
  { path: 'footer', component: FooterComponent },
  { path: 'contact-us', component: ContactUsComponent },
  {
    path: 'my-pages',
    component: MyPagesComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: 'start',
        component: MyPagesStartComponent
      },
      {
        path: 'projects/:id',
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
      },
      {
        path: 'edit-account',
        component: MyPagesEditAccountComponent
      },
      {
        path: 'delete-account',
        component: MyPagesDeleteAccountComponent
      },
      {
        path: 'administrate-base-activities',
        component: AdministrateBaseActivities
      },
      {
        path: 'administrate-base-activities-edit/:id',
        component: AdministrateBaseActivitiesEdit
      },
      {
        path: 'administrate-base-activities-new',
        component: AdministrateBaseActivitiesNew
      },
      {
        path: 'administrate-themes', 
        component: AdministrateThemes
      },
      {
        path: 'administrate-themes-edit/:id', 
        component: AdministrateThemesEdit
      },
      {
        path: 'administrate-themes-new', 
        component: AdministrateThemesNew
      },
      {
        path: 'administrate-users', /* http://localhost:5000/my-pages/administrate-users */
        component: AdministrateUsers
      }
    ]
  }

];

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    FooterComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    LoginComponent,
    HelpComponent,
    ContactUsComponent,
    MyPagesComponent,
    MyPagesStartComponent,
    MyPagesProjectComponent,
    MyPagesActivityStatusComponent,
    MyPagesActivityComponent,
    MyPagesMembersComponent,
    MyPagesProjectSettingsComponent,
    MyPagesEditAccountComponent,
    MyPagesDeleteAccountComponent,
    AdministrateBaseActivities,
    AdministrateBaseActivitiesEdit,
    AdministrateBaseActivitiesNew,
    AdministrateThemes,
    AdministrateThemesEdit,
    AdministrateThemesNew,
    AdministrateUsers,
    MatchesPhaseAndThemePipe,
    EnumToArrayPipe,
    NumberToPhasePipe,
    NumberToRolePipe,
    NumberToRightsPipe,
    NumberToStatusPipe,
    RegisterComponent,
    AlertComponent,
    JsonToDatePipe,
    AddActivityModal
  ],
  entryComponents: [
    AddActivityModal
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
   // ConnectionServiceModule,
    MarkdownModule.forRoot(),
    RouterModule.forRoot(routes, { anchorScrolling: 'enabled' })    
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    DatePipe
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
export const AppComponents = []
