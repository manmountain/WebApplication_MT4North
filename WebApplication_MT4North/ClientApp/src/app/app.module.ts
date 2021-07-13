import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule, Routes } from '@angular/router';
import { DatePipe } from '@angular/common';


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
import { MyPagesComponent } from './my-pages/my-pages.component';
import { MyPagesMembersComponent } from './my-pages-members/my-pages-members.component';
import { MyPagesProjectSettingsComponent } from './my-pages-project-settings/my-pages-project-settings.component';
import { MyPagesStartComponent } from './my-pages-start/my-pages-start.component';
import { MyPagesProjectComponent } from './my-pages-project/my-pages-project.component';
import { MyPagesActivityStatusComponent } from './my-pages-activity-status/my-pages-activity-status.component';
import { MyPagesActivityComponent } from './my-pages-activity/my-pages-activity.component';
import { MyPagesEditAccountComponent } from './my-pages-edit-account';
import { MatchesPhaseAndThemePipe, EnumToArrayPipe, NumberToPhasePipe, NumberToRolePipe, NumberToRightsPipe, NumberToStatusPipe, JsonToDatePipe } from './_models';
import { JwtInterceptor, ErrorInterceptor, AuthGuard } from './_helpers';
import { RegisterComponent } from './register/register.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'counter', component: CounterComponent },
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
    ContactUsComponent,
    MyPagesComponent,
    MyPagesStartComponent,
    MyPagesProjectComponent,
    MyPagesActivityStatusComponent,
    MyPagesActivityComponent,
    MyPagesMembersComponent,
    MyPagesProjectSettingsComponent,
    MyPagesEditAccountComponent,
    MatchesPhaseAndThemePipe,
    EnumToArrayPipe,
    NumberToPhasePipe,
    NumberToRolePipe,
    NumberToRightsPipe,
    NumberToStatusPipe,
    RegisterComponent,
    AlertComponent,
    JsonToDatePipe
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
   // ConnectionServiceModule,
    RouterModule.forRoot(routes)
    
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
