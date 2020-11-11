import { MyProfileComponent } from './my-profile/my-profile.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { IndexPage } from './index/index.page';


const routes: Routes = [
  { path: '', component: IndexPage },
  { path: 'Index', component: IndexPage },
  { path: 'MyProfile', component: MyProfileComponent},
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class UserRoutingModule { }
