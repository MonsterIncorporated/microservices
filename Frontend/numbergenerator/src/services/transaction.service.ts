import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { CreateTransactionDto } from '../dtos/create-transaction.dto';
import { TransactionDto } from '../dtos/transaction.dto';

@Injectable({
  providedIn: 'root',
})
export class TransactionService {
  constructor(private readonly httpClient: HttpClient) {}

  public async postAsync(createTransactionDto: CreateTransactionDto): Promise<TransactionDto> {
    return await firstValueFrom(
      this.httpClient.post<TransactionDto>(
        'http://localhost:8082/transaction',
        createTransactionDto,
      ),
    );
  }
}
