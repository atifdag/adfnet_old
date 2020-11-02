import { RegisterPage } from './register/register.page';
import { LoginPage } from './login/login.page';
import { AuthenticationRoutingModule } from './authentication-routing.module';
import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';



@NgModule({
  declarations: [
    LoginPage,
    RegisterPage
  ],
  imports: [
    SharedModule,
    AuthenticationRoutingModule
  ]
})
export class AuthenticationModule { }
