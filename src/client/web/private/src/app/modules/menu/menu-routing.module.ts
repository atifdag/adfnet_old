import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthenticationGuard } from 'src/app/authentication.guard';
import { ListComponent } from './list/list.component';

const routes: Routes = [
  { path: '', component: ListComponent, canActivate: [AuthenticationGuard] },
  //{ path: 'Add', component: AddComponent, canActivate: [AuthenticationGuard] },
  { path: 'List', component: ListComponent, canActivate: [AuthenticationGuard] },
  // { path: 'Update/:id', component: UpdateComponent, canActivate: [AuthenticationGuard] },
  // { path: 'Detail/:id', component: DetailComponent, canActivate: [AuthenticationGuard] },
  // { path: 'MyProfile', component: MyProfileComponent, canActivate: [AuthenticationGuard] },
  // { path: 'UpdateMyPassword', component: UpdateMyPasswordComponent, canActivate: [AuthenticationGuard] },
  // { path: 'UpdateMyInformation', component: UpdateMyInformationComponent, canActivate: [AuthenticationGuard] },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MenuRoutingModule { }
