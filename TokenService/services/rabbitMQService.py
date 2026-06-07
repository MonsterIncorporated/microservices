import os
import json
import threading
import pika

from models.transaction import TransactionStatus
from dtos.transaction import TransactionDto
from logger import logger


def get_credentials():
    return pika.PlainCredentials(
        os.environ["RABBIT_USER"],
        os.environ["RABBIT_PASS"],
    )


def get_connection():
    return pika.BlockingConnection(
        pika.ConnectionParameters(
            host=os.environ["RABBIT_HOST"],
            credentials=get_credentials(),
        )
    )


def on_number_generated(ch, method, properties, body):
    from services.transactionService import TransactionService

    try:
        message = json.loads(body.decode("utf-8"))
        transaction_id = message.get("transactionId")

        logger.info(
            f"Received number.generated for transactionID: {transaction_id}",
            extra={"filetype": "RabbitMQ"},
        )

        transactionDto = TransactionService.get_transaction(transaction_id)

        if transactionDto is None:
            logger.error(
                f"Transaction not found: {transaction_id}",
                extra={"filetype": "RabbitMQ"},
            )
            ch.basic_nack(delivery_tag=method.delivery_tag, requeue=False)
            return

        transactionDto.status = TransactionStatus.COMPLETED
        TransactionService.update_transaction(transactionDto)

        logger.info(
            f"Status updated for Transaction with ID: {transaction_id}",
            extra={"filetype": "RabbitMQ"},
        )

        ch.basic_ack(delivery_tag=method.delivery_tag)

    except Exception as e:
        logger.error(
            f"Failed to handle number.generated: {e}",
            extra={"filetype": "RabbitMQ"},
        )
        ch.basic_nack(delivery_tag=method.delivery_tag, requeue=False)


class RabbitMQService:
    @staticmethod
    def start_consuming():
        connection = get_connection()
        channel = connection.channel()

        channel.exchange_declare(
            exchange="success",
            exchange_type="direct",
            durable=True,
        )

        channel.queue_declare(
            queue="tokenservice",
            durable=True,
            arguments={"x-queue-type": "quorum"},
        )

        channel.queue_bind(
            exchange="success",
            queue="tokenservice",
            routing_key="number.generated",
        )

        channel.basic_consume(
            queue="tokenservice",
            on_message_callback=on_number_generated,
            auto_ack=False,
        )

        logger.info(
            "Started consuming number.generated",
            extra={"filetype": "RabbitMQ"},
        )

        channel.start_consuming()

    @staticmethod
    def start_consuming_thread():
        threading.Thread(
            target=RabbitMQService.start_consuming,
            daemon=True,
        ).start()

    @staticmethod
    def publish_token_pruchase(transactionDto: TransactionDto):
        connection = get_connection()
        channel = connection.channel()

        try:
            channel.exchange_declare(
                exchange="events",
                exchange_type="direct",
                durable=True,
            )

            channel.basic_publish(
                exchange="events",
                routing_key="token.purchased",
                body=transactionDto.model_dump_json().encode("utf-8"),
                properties=pika.BasicProperties(
                    content_type="application/json",
                    delivery_mode=2,
                ),
            )

            logger.info(
                "Sent Token Purchased with transactionID: " + transactionDto.transactionId,
                extra={"filetype": "RabbitMQ"},
            )

        finally:
            channel.close()
            connection.close()