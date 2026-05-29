import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'numbers',
    loadComponent: () => import('./numbervault/numbervault').then((m) => m.NumbervaultComponent),
  },
  {
    path: '',
    loadComponent: () => import('./overview/overview').then((m) => m.Overview),
  },
];
