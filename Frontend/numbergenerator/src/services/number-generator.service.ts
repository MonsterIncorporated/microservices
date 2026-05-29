import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { firstValueFrom } from "rxjs";

@Injectable({
  providedIn: 'root',
})
export class NumberGeneratorService {
  constructor(private readonly httpClient: HttpClient) {}

  public async getAsync(min: number, max: number): Promise<number> {
    return await firstValueFrom(
      this.httpClient.get<number>('http://localhost:8080/Number', { params: { min, max } }),
    );
  }
}