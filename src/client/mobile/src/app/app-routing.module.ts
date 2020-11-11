import { ErrorLayoutComponent } from './layouts/error-layout/error-layout.component';
import { PublicLayoutComponent } from './layouts/public-layout/public-layout.component';
import { PrivateLayoutComponent } from './layouts/private-layout/private-layout.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    component: PrivateLayoutComponent,
    children: [
      {
        path: '',
        loadChildren: () => import('./modules/home/home.module').then(m => m.HomeModule),
        pathMatch: 'full',
      },
      {
        path: 'Home',
        loadChildren: () => import('./modules/home/home.module').then(m => m.HomeModule),
      },
      {
        path: 'User',
        loadChildren: () => import('./modules/user/user.module').then(m => m.UserModule),
      }
    ]
  },
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      {
        path: 'Authentication',
        loadChildren: () => import('./modules/authentication/authentication.module').then(m => m.AuthenticationModule),
      },
    ]
  },
  {
    path: '',
    component: ErrorLayoutComponent,
    children: [
      {
        path: 'Error',
        loadChildren: () => import('./modules/error/error.module').then(m => m.ErrorModule),
      },
    ]
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
