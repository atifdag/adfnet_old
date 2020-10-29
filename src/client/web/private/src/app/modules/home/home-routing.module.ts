import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthenticationGuard } from 'src/app/authentication.guard';
import { IndexComponent } from './index/index.component';

const routes: Routes = [
  { path: '', component: IndexComponent, canActivate: [AuthenticationGuard] },
  { path: 'Index', component: IndexComponent, canActivate: [AuthenticationGuard] },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HomeRoutingModule { }
