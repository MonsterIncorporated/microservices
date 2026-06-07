from typing import Annotated, List
from fastapi import APIRouter, Depends, HTTPException
from services.transactionService import TransactionService
from dtos.error import ErrorDto
from oauth import oauth2_scheme
from logger import logger
from dtos.transaction import CreateTransactionDto, TransactionDto

router = APIRouter(prefix="/transaction", tags=["transaction"])

@router.get("/", response_model=TransactionDto, responses={404: {"model": ErrorDto}})
def get_transaction(transactionId: str, token: Annotated[str, Depends(oauth2_scheme)]):
    logger.info("Getting transaction with transactionId: " + transactionId, extra={"filetype": "transactionController"})
    transaction = TransactionService.get_transaction(transactionId)
    if(transaction == None):
        logger.info("Transaction with transactionId: " + transactionId + " not found", extra={"filetype": "transactionController"})
        raise HTTPException(status_code=404, detail={
            "name": "Not Found", 
            "detail": "Transaction with transactionId: " + transactionId + " not found", 
            "ofType": "TransactionDto", 
            "code": 404})
    logger.info("Transaction with transactionId: " + transactionId + " found", extra={"filetype": "transactionController"})
    return transaction

@router.get("/user/{userId}", response_model=List[TransactionDto])
def get_transaction_from_user(userId: str, token: Annotated[str, Depends(oauth2_scheme)]):
    logger.info("Getting transactions with userId: " + userId, extra={"filetype": "transactionController"})
    transactions = TransactionService.get_transaction_from_user(userId)
    logger.info("Transactions with userId: " + userId + " found: " + str(len(transactions)), extra={"filetype": "transactionController"})
    return transactions

@router.post("/", response_model=TransactionDto)
def create_transaction(createTransactionDto: CreateTransactionDto, token: Annotated[str, Depends(oauth2_scheme)]):
    logger.info("Creating transaction with userId: " + createTransactionDto.userId, extra={"filetype": "transactionController"})
    transactionDto = TransactionService.create_transaction(createTransactionDto)
    logger.info("Transaction with userId: " + createTransactionDto.userId + " created successfully with transactionId: " + transactionDto.transactionId, extra={"filetype": "transactionController"})
    return transactionDto

@router.put("/", response_model=TransactionDto, responses={404: {"model": ErrorDto}})
def update_transaction(transactionDto: TransactionDto, token: Annotated[str, Depends(oauth2_scheme)]):
    logger.info("Updating transaction with transactionId: " + transactionDto.transactionId, extra={"filetype": "transactionController"})
    newTransactionDto = TransactionService.update_transaction(transactionDto)
    if(newTransactionDto == None):
        logger.info("Transaction with transactionId: " + transactionDto.transactionId + " not found", extra={"filetype": "transactionController"})
        raise HTTPException(status_code=404, detail={
            "name": "Not Found", 
            "detail": "Transaction with userId: " + newTransactionDto.transactionId + " not found", 
            "ofType": "TransactionDto", 
            "code": 404})
    logger.info("Transaction with transactionId: " + transactionDto.transactionId + " updated successfully", extra={"filetype": "transactionController"})
    return newTransactionDto