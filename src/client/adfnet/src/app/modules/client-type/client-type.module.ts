import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IndexComponent } from './index/index.component';
import { AddComponent } from './add/add.component';
import { SharedModule } from 'src/app/shared.module';
import { ClientTypeRoutingModule } from './client-type-routing.module';

@NgModule({
  declarations: [IndexComponent, AddComponent],
  imports: [
    CommonModule,
    SharedModule,
    ClientTypeRoutingModule
  ]
})
export class ClientTypeModule { }
