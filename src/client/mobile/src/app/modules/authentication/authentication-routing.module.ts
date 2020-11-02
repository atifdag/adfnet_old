import { RegisterPage } from './register/register.page';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginPage } from './login/login.page';


const routes: Routes = [
  { path: '', component: LoginPage },
  { path: 'Login', component: LoginPage },
  { path: 'Register', component: RegisterPage },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AuthenticationRoutingModule { }
