import { IndexPage } from './../home/index/index.page';
import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { MyProfilePage } from './my-profile/my-profile.page';
import { UserRoutingModule } from './user-routing.module';


@NgModule({
  declarations: [
    MyProfilePage,
    IndexPage
  ],
  imports: [
    SharedModule,
    UserRoutingModule
  ]
})
export class UserModule { }
