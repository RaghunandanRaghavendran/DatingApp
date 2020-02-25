import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListsComponent } from './member-lists/member-lists.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './guards/auth.guard';

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
       {path: 'messages', component: MessagesComponent},
       {path: 'lists', component: ListsComponent}
    ]
},

{path: '**', redirectTo: '', pathMatch: 'full'}

];
