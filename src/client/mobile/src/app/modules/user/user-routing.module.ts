import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { IndexPage } from './index/index.page';
import { MyProfilePage } from './my-profile/my-profile.page';


const routes: Routes = [
  { path: '', component: IndexPage },
  { path: 'Index', component: IndexPage },
  { path: 'MyProfile', component: MyProfilePage },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class UserRoutingModule { }
