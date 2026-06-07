from dtos.wallet import WalletDto
from services.rabbitMQService import RabbitMQService
from services.walletService import WalletService
from dtos.transaction import TransactionDto, CreateTransactionDto
from db.db import db
import uuid
from models.transaction import Transaction


class TransactionService:
    @staticmethod
    def get_transaction_from_user(userId: str):
        transactions = db.query(Transaction).filter(Transaction.userId == userId).all()
        return TransactionDto.to_dtos(transactions)

    @staticmethod
    def get_transaction(transactionId: str):
        transaction = db.query(Transaction).filter(Transaction.transactionId == transactionId).first()
        if(transaction == None):
            return None
        return TransactionDto.to_dto(transaction)
    
    @staticmethod
    def create_transaction(transactionDto: CreateTransactionDto):
        walletDto = WalletService.get_wallet(transactionDto.userId);

        if(walletDto.tokens >= transactionDto.requiredTokens):
            WalletService.update_wallet(WalletDto(userId=walletDto.userId, tokens=walletDto.tokens - transactionDto.requiredTokens));
            transaction = Transaction(
            transactionId=str(uuid.uuid4()), 
            userId=transactionDto.userId, 
            requiredTokens=transactionDto.requiredTokens, 
            numberOfDigits=transactionDto.numberOfDigits, 
            status=transactionDto.status)
            db.add(transaction)
            db.commit()

            RabbitMQService.publish_token_pruchase(TransactionDto.to_dto(transaction));
            return TransactionDto.to_dto(transaction)
        else:
            return None
        
    @staticmethod
    def update_transaction(transactionDto: TransactionDto):
        transactionFromDb = db.query(Transaction).filter(Transaction.transactionId == transactionDto.transactionId).first()
        if transactionFromDb == None:
            return None

        transactionFromDb.status = transactionDto.status
        db.commit()

        return TransactionDto.to_dto(transactionFromDb)