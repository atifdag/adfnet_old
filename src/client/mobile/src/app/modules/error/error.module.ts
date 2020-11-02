import { ErrorRoutingModule } from './error-routing.module';
import { ServerPage } from './server/server.page';
import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';



@NgModule({
  declarations: [
    ServerPage
  ],
  imports: [
    SharedModule,
    ErrorRoutingModule
  ]
})
export class ErrorModule { }
