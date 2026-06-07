import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { from, switchMap } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);

  const isApi8080 = req.url.includes('/api') && req.url.includes('8080');

  const isWallet8082 = req.url.includes('/wallet') && req.url.includes('8082');

  const isNumber8080 = req.url.includes('/Number') && req.url.includes('8080');

  const isTransaction8082 = req.url.includes('/transaction') && req.url.includes('8082');

  if (!isApi8080 && !isWallet8082 && !isNumber8080 && !isTransaction8082) {
    return next(req);
  }

  return from(auth.updateToken()).pipe(
    switchMap((token) => {
      if (!token) return next(req);

      return next(
        req.clone({
          setHeaders: {
            Authorization: `Bearer ${token}`,
          },
        }),
      );
    }),
  );
};
