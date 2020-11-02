import { PrivateLayoutPage } from './layouts/private-layout/private-layout.page';
import { NgModule } from '@angular/core';
import { RouterModule, Routes, PreloadAllModules } from '@angular/router';
import { PublicLayoutPage } from './layouts/public-layout/public-layout.page';
import { ErrorLayoutPage } from './layouts/error-layout/error-layout.page';
import { CustomPreloaderStrategy } from './custom-preloader-strategy';

const routes: Routes = [
  {
    path: '',
    component: PrivateLayoutPage,
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
    component: PublicLayoutPage,
    children: [
      {
        path: 'Authentication',
        loadChildren: () => import('./modules/authentication/authentication.module').then(m => m.AuthenticationModule),
      },
    ]
  },
  {
    path: '',
    component: ErrorLayoutPage,
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
