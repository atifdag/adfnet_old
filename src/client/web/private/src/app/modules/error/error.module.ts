import { NgModule } from '@angular/core';
import { ServerComponent } from './server/server.component';
import { ErrorRoutingModule } from './error-routing.module';
import { SharedModule } from '../shared/shared.module';


@NgModule({
  declarations: [ServerComponent],
  imports: [
    SharedModule,
    ErrorRoutingModule
  ]
})
export class ErrorModule { }
