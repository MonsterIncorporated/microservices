import { Inject, Injectable } from '@angular/core';
import Connection, { Publisher } from 'rabbitmq-client';

@Injectable({
  providedIn: 'root',
})
export class RabbitMQSender {
  private rabbit: Connection;
  private publisher: Publisher;
  constructor() {
    this.rabbit = new Connection('amqp://password@rabbitmq:5672');
    this.publisher = this.rabbit.createPublisher();
  }
  public sendMesage() {
    this.publisher.send(
      {
        headers: { type: 'get', transactionId: '67' },
        routingKey: 'token.purchased',
        exchange: 'events',
      },
      'sent from frontend',
    );
  }
}
