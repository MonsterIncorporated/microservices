import { Component } from '@angular/core';
import { NavbarComponent } from "../navbar/navbar";
import { NumberGeneratorService } from '../../services/number-generator.service';
import { NumberfieldComponent } from "../numberfield/numberfield";

@Component({
  selector: 'app-numbervault-component',
  imports: [NavbarComponent, NumberfieldComponent],
  templateUrl: './numbervault.html',
  styleUrl: './numbervault.css',
})
export class NumbervaultComponent {
  protected number?: number;
  public constructor(private readonly numberGeneratoService: NumberGeneratorService) {}
  protected async getNumber() {
    this.number = await this.numberGeneratoService.getAsync(50, 100);
  }
}
