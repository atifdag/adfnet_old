import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared.module';
import { ListComponent } from './list/list.component';
import { MenuRoutingModule } from './menu-routing.module';



@NgModule({
  declarations: [
    ListComponent
  ],
  imports: [
    SharedModule,
    MenuRoutingModule
  ]
})
export class MenuModule { }
