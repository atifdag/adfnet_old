import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ServerPage } from './server/server.page';


const routes: Routes = [
  { path: '', component: ServerPage },
  { path: 'Server', component: ServerPage }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ErrorRoutingModule { }
