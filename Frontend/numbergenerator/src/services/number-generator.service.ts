import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { GeneratedNumberDto } from '../dtos/generated-number.dto';
import { CreateGeneratedNumberDto } from '../dtos/create-generated-number.dto';

@Injectable({
  providedIn: 'root',
})
export class NumberGeneratorService {
  constructor(private readonly httpClient: HttpClient) {}

  public async postAsync(
    createGeneratedNumberDto: CreateGeneratedNumberDto,
  ): Promise<GeneratedNumberDto> {
    return await firstValueFrom(
      this.httpClient.post<GeneratedNumberDto>(
        'http://localhost:8080/Number',
        createGeneratedNumberDto,
      ),
    );
  }

  public async getNumbersAsync(userId: string): Promise<GeneratedNumberDto[]> {
    return await firstValueFrom(
      this.httpClient.get<GeneratedNumberDto[]>('http://localhost:8080/Number', {
        params: { userId: userId },
      }),
    );
  }

  public async deleteNumberAsync(numberId: string): Promise<GeneratedNumberDto[]> {
    return await firstValueFrom(
      this.httpClient.delete<GeneratedNumberDto[]>(`http://localhost:8080/Number/${numberId}`),
    );
  }
}
