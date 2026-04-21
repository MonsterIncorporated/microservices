import { Component } from '@angular/core';
import { NumberGeneratorService } from '../../services/number-generator.service';

@Component({
  selector: 'app-overview',
  imports: [],
  templateUrl: './overview.html',
  styleUrl: './overview.css',
})
export class Overview {
  public constructor(private readonly numberGeneratoService: NumberGeneratorService) {}

  protected async getNumber() {
    var numbers = await this.numberGeneratoService.getAsync();
    console.log(numbers);
  }
}
