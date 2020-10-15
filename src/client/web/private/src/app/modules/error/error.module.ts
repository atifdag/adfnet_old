import { NgModule } from '@angular/core';
import { ServerComponent } from './server/server.component';
import { SharedModule } from 'src/app/shared.module';
import { ErrorRoutingModule } from './error-routing.module';



@NgModule({
  declarations: [ServerComponent],
  imports: [
    SharedModule,
    ErrorRoutingModule
  ]
})
export class ErrorModule { }
