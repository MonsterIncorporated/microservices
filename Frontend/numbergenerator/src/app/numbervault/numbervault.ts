import { Component, OnInit, signal } from '@angular/core';
import { NavbarComponent } from '../navbar/navbar';
import { NumberGeneratorService } from '../../services/number-generator.service';
import { NumberfieldComponent } from '../numberfield/numberfield';
import { GeneratedNumberDto } from '../../dtos/generated-number.dto';
import { CreateGeneratedNumberDto } from '../../dtos/create-generated-number.dto';
import { form, FormField, max, min } from '@angular/forms/signals';

@Component({
  selector: 'app-numbervault-component',
  imports: [NavbarComponent, NumberfieldComponent, FormField],
  templateUrl: './numbervault.html',
  styleUrl: './numbervault.css',
})
export class NumbervaultComponent implements OnInit {
  protected numbergeneratorForm = form(signal({
    digits: 5
  }),(schemaPath) => {
    min(schemaPath.digits, 1, {message: 'Digits must be at least 1'});
    max(schemaPath.digits, 7, {message: 'Digits must be at most 7'});
  });
  protected amount = 12;
  protected randomNumber = signal(0);
  protected loadingNumber = false;

  protected number(){
    return Math.pow(10,this.numbergeneratorForm().controlValue().digits) - 1;
  }
  protected numbers = signal<GeneratedNumberDto[]>([]);

  public constructor(private readonly numberGeneratoService: NumberGeneratorService) {}

  ngOnInit(): void {
    this.getNumbersAsync();
  }

  protected async deleteNumberAsync(numberId: string) {
    await this.numberGeneratoService.deleteNumberAsync(numberId);
    this.numbers.set(this.numbers().filter((n) => n.id !== numberId));
  }

  private async getNumbersAsync() {
    this.numbers.set(
      await this.numberGeneratoService.getNumbersAsync('123e4567-e89b-12d3-a456-426655440000'),
    );
  }

  protected async postAsync() {
    this.loadingNumber = true;
    var intervalId = this.startRandomNumberAnimation();

    var generatedNumberDto = await this.numberGeneratoService.postAsync({
      userId: '123e4567-e89b-12d3-a456-426655440000',
      min: Math.pow(10, this.numbergeneratorForm().controlValue().digits - 1),
      max: Math.pow(10, this.numbergeneratorForm().controlValue().digits) - 1,
    } as CreateGeneratedNumberDto);
    //this.number = generatedNumberDto.value;

    await this.delay(1000);

    this.stopRandomNumberAnimation(intervalId);
    this.loadingNumber = false;

    this.getNumbersAsync();
  }

  private startRandomNumberAnimation(): number {
    const intervalId = setInterval(() => {
      this.randomNumber.set(Math.pow(10, this.numbergeneratorForm().controlValue().digits - 1) + Math.round(Math.random() * (Math.pow(10, this.numbergeneratorForm().controlValue().digits) - Math.pow(10, this.numbergeneratorForm().controlValue().digits - 1) - 1)));
    }, 100);
    return intervalId;
  }

  private stopRandomNumberAnimation(intervalId: number) {
    clearInterval(intervalId);
    this.randomNumber.set(0);
  }

  private async delay(ms: number) {
    return new Promise((resolve) => setTimeout(resolve, ms));
  }
}
