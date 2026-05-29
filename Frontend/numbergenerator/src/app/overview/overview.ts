import { Component, OnInit, signal } from '@angular/core';
import { NavbarComponent } from '../navbar/navbar';
import { NumberfieldComponent } from '../numberfield/numberfield';
import { interval } from 'rxjs';

@Component({
  selector: 'app-overview',
  imports: [NavbarComponent, NumberfieldComponent],
  templateUrl: './overview.html',
  styleUrl: './overview.css',
})
export class Overview implements OnInit {
  protected index = signal(0);
  protected overviewNumbers = [6456, 5329, 1781, 2194, 9872, 3947];

  ngOnInit(): void {
    interval(200).subscribe(() => {
      var number = Math.round(Math.random() * 5);
      if (number == this.index()) {
        number = (number + 1) % 6;
      }
      this.index.set(number);
    });
  }
}
