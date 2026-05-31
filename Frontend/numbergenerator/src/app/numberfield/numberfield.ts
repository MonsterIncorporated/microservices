import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-numberfield-component',
  imports: [],
  templateUrl: './numberfield.html',
  styleUrl: './numberfield.css',
})
export class NumberfieldComponent {
  @Input({ required: true }) numberstring: string = '0';
}
