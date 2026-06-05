import { Component, OnInit } from '@angular/core';
import { NavbarComponent } from '../navbar/navbar';
import { AuthService } from '../../services/auth.service';
import { UserDto } from '../../dtos/user';

@Component({
  selector: 'app-profile-component',
  imports: [NavbarComponent],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class ProfileComponent implements OnInit {
  protected user?: UserDto;

  constructor(private readonly authService: AuthService) {}
  ngOnInit(): void {
    this.user = this.authService.getUser();
    console.log(this.user.name);
  }
  logout() {
    this.authService.logout();
  }
}
