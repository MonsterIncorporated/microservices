import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-navbar-component',
  imports: [],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class NavbarComponent {
  protected tokens = 128;

  constructor(private readonly authService: AuthService) {}

  protected authenticated() {
    return this.authService.authenticated();
  }
}
