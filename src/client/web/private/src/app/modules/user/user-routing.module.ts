import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthenticationGuard } from 'src/app/authentication.guard';
import { AddComponent } from './add/add.component';
import { DetailComponent } from './detail/detail.component';
import { ListComponent } from './list/list.component';
import { MyProfileComponent } from './my-profile/my-profile.component';
import { UpdateMyInformationComponent } from './update-my-information/update-my-information.component';
import { UpdateMyPasswordComponent } from './update-my-password/update-my-password.component';
import { UpdateComponent } from './update/update.component';

const routes: Routes = [
  { path: '', component: ListComponent, canActivate: [AuthenticationGuard] },
  { path: 'Add', component: AddComponent, canActivate: [AuthenticationGuard] },
  { path: 'List', component: ListComponent, canActivate: [AuthenticationGuard] },
  { path: 'Update/:id', component: UpdateComponent, canActivate: [AuthenticationGuard] },
  { path: 'Detail/:id', component: DetailComponent, canActivate: [AuthenticationGuard] },
  { path: 'MyProfile', component: MyProfileComponent, canActivate: [AuthenticationGuard] },
  { path: 'UpdateMyPassword', component: UpdateMyPasswordComponent, canActivate: [AuthenticationGuard] },
  { path: 'UpdateMyInformation', component: UpdateMyInformationComponent, canActivate: [AuthenticationGuard] },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserRoutingModule { }
