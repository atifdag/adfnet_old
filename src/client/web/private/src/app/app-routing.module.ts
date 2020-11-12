import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ErrorLayoutComponent } from './layouts/error-layout/error-layout.component';
import { PrivateLayoutComponent } from './layouts/private-layout/private-layout.component';
import { PublicLayoutComponent } from './layouts/public-layout/public-layout.component';

const routes: Routes = [
  {
    path: '',
    component: PrivateLayoutComponent,
    children: [
      {
        path: '',
        loadChildren: () => import('./modules/home/home.module').then(m => m.HomeModule),
        pathMatch: 'full'
      },
      {
        path: 'Home',
        loadChildren: () => import('./modules/home/home.module').then(m => m.HomeModule)
      },
      {
        path: 'User',
        loadChildren: () => import('./modules/user/user.module').then(m => m.UserModule)
      },
      {
        path: 'Category',
        loadChildren: () => import('./modules/category/category.module').then(m => m.CategoryModule)
      },

    ]
  },
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      {
        path: 'Authentication',
        loadChildren: () => import('./modules/authentication/authentication.module').then(m => m.AuthenticationModule)
      },
    ]
  },
  {
    path: '',
    component: ErrorLayoutComponent,
    children: [
      {
        path: 'Error',
        loadChildren: () => import('./modules/error/error.module').then(m => m.ErrorModule)
      },
    ]
  },
];


@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
