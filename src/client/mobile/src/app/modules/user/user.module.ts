import { MyProfileComponent } from './my-profile/my-profile.component';
import { IndexPage } from './index/index.page';
import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { UserRoutingModule } from './user-routing.module';


@NgModule({
  declarations: [
    MyProfileComponent,
    IndexPage
  ],
  imports: [
    SharedModule,
    UserRoutingModule
  ]
})
export class UserModule { }
