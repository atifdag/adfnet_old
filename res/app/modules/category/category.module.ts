import { SharedModule } from './../../shared.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CategoryRoutingModule } from './category-routing.module';
import { IndexComponent } from './index/index.component';
import { AddComponent } from './add/add.component';


@NgModule({
  declarations: [IndexComponent, AddComponent],
  imports: [
    CommonModule,
    SharedModule,
    CategoryRoutingModule
  ]
})
export class CategoryModule { }
