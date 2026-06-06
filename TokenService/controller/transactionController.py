from typing import List
from fastapi import APIRouter, HTTPException
from services.transactionService import TransactionService
from dtos.error import ErrorDto
from dtos.transaction import CreateTransactionDto, TransactionDto

router = APIRouter(prefix="/transaction", tags=["transaction"])

@router.get("/", response_model=TransactionDto, responses={404: {"model": ErrorDto}})
def get_transaction(transactionId: str):
    transaction = TransactionService.get_transaction(transactionId)
    if(transaction == None):
        raise HTTPException(status_code=404, detail={
            "name": "Not Found", 
            "detail": "Transaction with transactionId: " + transactionId + " not found", 
            "ofType": "TransactionDto", 
            "code": 404})
    return transaction

@router.get("/user/{userId}", response_model=List[TransactionDto])
def get_transaction_from_user(userId: str):
    transactions = TransactionService.get_transaction_from_user(userId)
    return transactions

@router.post("/", response_model=TransactionDto)
def create_transaction(createTransactionDto: CreateTransactionDto):
    transactionDto = TransactionService.create_transaction(createTransactionDto)
    return transactionDto

@router.put("/", response_model=TransactionDto, responses={404: {"model": ErrorDto}})
def update_transaction(transactionDto: TransactionDto):
    newTransactionDto = TransactionService.update_transaction(transactionDto)
    if(newTransactionDto == None):
        raise HTTPException(status_code=404, detail={
            "name": "Not Found", 
            "detail": "Transaction with userId: " + newTransactionDto.transactionId + " not found", 
            "ofType": "TransactionDto", 
            "code": 404})
    return newTransactionDto