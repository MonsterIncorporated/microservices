import { Routes } from '@angular/router';
import { authGuard } from '../auth/auth.guard';

export const routes: Routes = [
  {
    path: 'numbers',
    loadComponent: () => import('./numbervault/numbervault').then((m) => m.NumbervaultComponent),
    canActivate: [authGuard]
  },
  {
    path: '',
    loadComponent: () => import('./overview/overview').then((m) => m.Overview),
  },
];
