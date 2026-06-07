import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { NavbarComponent } from '../navbar/navbar';
import { AuthService } from '../../services/auth.service';
import { UserDto } from '../../dtos/user';
import { WalletService } from '../../services/token.service';
import { WalletDto } from '../../dtos/wallet.dto';

@Component({
  selector: 'app-profile-component',
  imports: [NavbarComponent],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class ProfileComponent implements OnInit {
  protected user?: UserDto;
  protected wallet?: WalletDto;

  constructor(
    private readonly authService: AuthService,
    private readonly walletService: WalletService,
    private readonly cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    void this.load();
  }

  private async load() {
    this.user = this.authService.getUser();
    this.wallet = await this.walletService.getWalletAsync(this.user.id);
    console.log(this.wallet.tokens);
    this.cdr.detectChanges();
  }

  logout() {
    this.authService.logout();
  }
}
