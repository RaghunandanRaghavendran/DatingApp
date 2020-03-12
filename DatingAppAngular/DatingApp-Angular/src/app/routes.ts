import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListsComponent } from './members/member-lists/member-lists.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';

export const appRoutes: Routes = [
{path: '', component: HomeComponent},

// Below code works fine, but the better way of creating the private route is in below
// {path: 'members', component: MemberListsComponent, canActivate: [AuthGuard]},
// {path: 'messages', component: MessagesComponent, canActivate: [AuthGuard]},
// {path: 'lists', component: ListsComponent,canActivate: [AuthGuard]},

{
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children : [
       {path: 'members', component: MemberListsComponent},
       {path: 'members/:id', component: MemberDetailsComponent,
        resolve: {user: MemberDetailResolver}},
       {path: 'messages', component: MessagesComponent},
       {path: 'lists', component: ListsComponent}
    ]
},

{path: '**', redirectTo: '', pathMatch: 'full'}

];
