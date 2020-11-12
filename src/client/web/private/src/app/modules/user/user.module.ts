import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddComponent } from './add/add.component';
import { UpdateComponent } from './update/update.component';
import { DetailComponent } from './detail/detail.component';
import { MyProfileComponent } from './my-profile/my-profile.component';
import { UpdateMyPasswordComponent } from './update-my-password/update-my-password.component';
import { UpdateMyInformationComponent } from './update-my-information/update-my-information.component';
import { ListComponent } from './list/list.component';
import { UserRoutingModule } from './user-routing.module';
import { SharedModule } from '../shared/shared.module';



@NgModule({
  declarations: [
    AddComponent,
    UpdateComponent,
    DetailComponent,
    MyProfileComponent,
    UpdateMyPasswordComponent,
    UpdateMyInformationComponent,
    ListComponent
  ],
  imports: [
    SharedModule,
    UserRoutingModule
  ]
})
export class UserModule { }
