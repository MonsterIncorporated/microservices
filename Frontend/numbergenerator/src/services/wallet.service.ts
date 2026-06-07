import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { WalletDto } from '../dtos/wallet.dto';
import { CreateWalletDto } from '../dtos/create-wallet.dto';

@Injectable({
  providedIn: 'root',
})
export class WalletService {
  constructor(private readonly httpClient: HttpClient) {}

  public async postAsync(createWalletDto: CreateWalletDto): Promise<WalletDto> {
    return await firstValueFrom(
      this.httpClient.post<WalletDto>('http://localhost:8082/wallet', createWalletDto),
    );
  }

  public async getWalletAsync(userId: string): Promise<WalletDto> {
    return await firstValueFrom(
      this.httpClient.get<WalletDto>('http://localhost:8082/wallet', {
        params: { userId: userId },
      }),
    );
  }
}
