// External imports
import { BrowserModule, HammerGestureConfig, HAMMER_GESTURE_CONFIG } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {HttpClientModule} from '@angular/common/http';
import {FormsModule} from '@angular/forms';
import { BsDropdownModule, TabsModule } from 'ngx-bootstrap';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { appRoutes } from './routes';
import { JwtModule } from '@auth0/angular-jwt';
import {NgxGalleryModule} from 'ngx-gallery';

// Services and Interceptors
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { AuthService } from './_services/auth.service';
import { AlertifyService } from './_services/alertify.service';

// Components created
import { AppComponent } from './app.component';
import { ValueComponent } from './value/value.component';
import { NavComponent } from './nav/nav.component';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { MemberListsComponent } from './members/member-lists/member-lists.component';
import { MembersComponent } from './members/members.component';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { AuthGuard } from './_guards/auth.guard';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';


export function tokenGetter() {
   return localStorage.getItem('token');
}

export class CustomHammerConfig extends HammerGestureConfig  {
   overrides = {
       pinch: { enable: false },
       rotate: { enable: false }
   };
}

@NgModule({
   declarations: [
      AppComponent,
      ValueComponent,
      NavComponent,
      HomeComponent,
      RegisterComponent,
      MessagesComponent,
      ListsComponent,
      MemberListsComponent,
      MembersComponent,
      MemberCardComponent,
      MemberDetailsComponent,
      MemberEditComponent
   ],
   imports: [
      BrowserModule,
      HttpClientModule,
      FormsModule,
      BrowserAnimationsModule,
      BsDropdownModule.forRoot(),
      TabsModule.forRoot(),
      RouterModule.forRoot(appRoutes),
      NgxGalleryModule,
      JwtModule.forRoot({
         config: {
            tokenGetter,
            whitelistedDomains: ['localhost:5000'],
            blacklistedRoutes: ['localhost:5000/auth']

         }
      })
   ],
   providers: [
      ErrorInterceptorProvider,
      AuthService,
      AlertifyService,
      AuthGuard,
      MemberDetailResolver,
      MemberEditResolver,
      PreventUnsavedChanges,
      { provide: HAMMER_GESTURE_CONFIG, useClass: CustomHammerConfig }
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
