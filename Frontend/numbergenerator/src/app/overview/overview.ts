import { Component } from '@angular/core';
import { Connection } from 'rabbitmq-client';
import { RabbitMQSender } from '../../rabbitmq/send';

@Component({
  selector: 'app-overview',
  imports: [],
  templateUrl: './overview.html',
  styleUrl: './overview.css',
})
export class Overview {
  public constructor(private readonly rabbitMqService: RabbitMQSender) {}

  protected getNumber() {
    this.rabbitMqService.sendMesage();
  }
}
