from typing import List

from pydantic import BaseModel
from models.transaction import Transaction, TransactionStatus

class TransactionDto(BaseModel):
    transactionId: str
    userId: str
    requiredTokens: int
    numberOfDigits: int
    status: TransactionStatus

    model_config = {
        "from_attributes": True
    } 

    @staticmethod
    def to_dtos(transactions: List[Transaction]):
        transactionDtos: List[TransactionDto] = []
        for transaction in transactions:
            transactionDtos.append(TransactionDto.to_dto(transaction))
        return transactionDtos

    @staticmethod
    def to_dto(transaction: Transaction):
        return TransactionDto(transactionId=transaction.transactionId, userId=transaction.userId, requiredTokens=transaction.requiredTokens, numberOfDigits=transaction.numberOfDigits, status=transaction.status)
    
    @staticmethod
    def to_entity(transactionDto: TransactionDto):
        return Transaction(transactionId=transactionDto.transactionId, userId=transactionDto.userId, requiredTokens=transactionDto.requiredTokens, numberOfDigits=transactionDto.numberOfDigits, status=transactionDto.status)
    
class CreateTransactionDto(BaseModel):
    userId: str
    requiredTokens: int
    numberOfDigits: int
    status: TransactionStatus

    model_config = {
        "from_attributes": True
    }
    
    @staticmethod
    def to_entity(transactionDto):
        return Transaction(transactionId="", userId=transactionDto.userId, requiredTokens=transactionDto.requiredTokens, numberOfDigits=transactionDto.numberOfDigits, status=transactionDto.status)