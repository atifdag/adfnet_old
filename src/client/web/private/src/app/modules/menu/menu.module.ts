import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared.module';
import { ListComponent } from './list/list.component';
import { MenuRoutingModule } from './menu-routing.module';
import { DetailComponent } from './detail/detail.component';
import { AddComponent } from './add/add.component';
import { UpdateComponent } from './update/update.component';



@NgModule({
  declarations: [
    ListComponent,
    DetailComponent,
    AddComponent,
    UpdateComponent
  ],
  imports: [
    SharedModule,
    MenuRoutingModule
  ]
})
export class MenuModule { }
