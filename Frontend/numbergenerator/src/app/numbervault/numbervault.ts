import { Component, OnInit, signal } from '@angular/core';
import { NavbarComponent } from '../navbar/navbar';
import { NumberGeneratorService } from '../../services/number-generator.service';
import { NumberfieldComponent } from '../numberfield/numberfield';
import { GeneratedNumberDto } from '../../dtos/generated-number.dto';
import { CreateGeneratedNumberDto } from '../../dtos/create-generated-number.dto';
import { form, FormField, max, min } from '@angular/forms/signals';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-numbervault-component',
  imports: [NavbarComponent, NumberfieldComponent, FormField],
  templateUrl: './numbervault.html',
  styleUrl: './numbervault.css',
})
export class NumbervaultComponent implements OnInit {
  setRandomNumber() {
    this.randomNumber.set(
      (Math.pow(10, this.numbergeneratorForm().controlValue().digits) - 1)
        .toString()
        .replaceAll('9', '?'),
    );
  }
  protected amount = 12;
  protected randomNumber = signal('?????');
  protected loadingNumber = false;
  protected numbers = signal<GeneratedNumberDto[]>([]);
  protected numbergeneratorForm = form(
    signal({
      digits: 5,
    }),
    (schemaPath) => {
      min(schemaPath.digits, 1, { message: 'Digits must be at least 1' });
      max(schemaPath.digits, 7, { message: 'Digits must be at most 7' });
    },
  );

  protected number() {
    return Math.pow(10, this.numbergeneratorForm().controlValue().digits) - 1;
  }

  public constructor(private readonly numberGeneratoService: NumberGeneratorService, private readonly authService: AuthService) {}

  ngOnInit(): void {
    this.getNumbersAsync();
  }

  protected async deleteNumberAsync(numberId: string) {
    await this.numberGeneratoService.deleteNumberAsync(numberId);
    this.numbers.set(this.numbers().filter((n) => n.id !== numberId));
  }

  private async getNumbersAsync() {
    this.numbers.set(
      await this.numberGeneratoService.getNumbersAsync(this.authService.getUserId()!),
    );
  }

  protected async postAsync() {
    this.loadingNumber = true;
    var intervalId = this.startRandomNumberAnimation();

    const userId = this.authService.getUserId();

    var generatedNumberDto = await this.numberGeneratoService.postAsync({
      userId: userId,
      min: Math.pow(10, this.numbergeneratorForm().controlValue().digits - 1),
      max: Math.pow(10, this.numbergeneratorForm().controlValue().digits) - 1,
    } as CreateGeneratedNumberDto);

    await this.delay(1000);

    await this.stopRandomNumberAnimation(intervalId);
    this.randomNumber.set(generatedNumberDto.value.toString());
    this.loadingNumber = false;

    this.getNumbersAsync();
  }

  private startRandomNumberAnimation(): number {
    const intervalId = setInterval(() => {
      this.randomNumber.set(
        (
          Math.pow(10, this.numbergeneratorForm().controlValue().digits - 1) +
          Math.round(
            Math.random() *
              (Math.pow(10, this.numbergeneratorForm().controlValue().digits) -
                Math.pow(10, this.numbergeneratorForm().controlValue().digits - 1) -
                1),
          )
        ).toString(),
      );
    }, 100);
    return intervalId;
  }

  private async stopRandomNumberAnimation(intervalId: number) {
    clearInterval(intervalId);
    this.randomNumber.set('0');
  }

  private async delay(ms: number) {
    return new Promise((resolve) => setTimeout(resolve, ms));
  }
}
