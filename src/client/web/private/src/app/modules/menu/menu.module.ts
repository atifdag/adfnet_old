import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared.module';
import { ListComponent } from './list/list.component';
import { MenuRoutingModule } from './menu-routing.module';
import { DetailComponent } from './detail/detail.component';



@NgModule({
  declarations: [
    ListComponent,
    DetailComponent
  ],
  imports: [
    SharedModule,
    MenuRoutingModule
  ]
})
export class MenuModule { }
