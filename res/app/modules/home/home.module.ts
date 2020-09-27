import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeRoutingModule } from './home-routing.module';
import { IndexComponent } from './index/index.component';
import { SharedModule } from 'src/app/shared.module';


@NgModule({
  declarations: [IndexComponent],
  imports: [
    CommonModule,
    SharedModule,
    HomeRoutingModule
  ]
})
export class HomeModule { }
