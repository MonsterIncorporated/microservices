import { Injectable, signal } from '@angular/core';
import Keycloak from 'keycloak-js';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private keycloak = new Keycloak({
    url: 'http://localhost:8081',
    realm: 'numbergenerator',
    clientId: 'numbergenerator-api-frontend'
  });

  authenticated = signal(false);

  async init(): Promise<void> {
    const loggedIn = await this.keycloak.init({
      onLoad: 'check-sso',
      pkceMethod: 'S256',
      checkLoginIframe: false
    });

    this.authenticated.set(loggedIn);
  }

  login() {
    return this.keycloak.login();
  }

  logout() {
    return this.keycloak.logout({
      redirectUri: "http://localhost/numbers"
    });
  }

  getToken(): string | undefined {
    return this.keycloak.token;
  }

  async updateToken(): Promise<string | undefined> {
    await this.keycloak.updateToken(30);
    return this.keycloak.token;
  }

  getUserId(): string | undefined {
    return this.keycloak.tokenParsed?.sub;
 }
}