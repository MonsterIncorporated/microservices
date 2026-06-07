from typing import Annotated
from oauth import oauth2_scheme
from fastapi import APIRouter, Depends, HTTPException
from logger import logger
from dtos.success import SuccessDto
from dtos.error import ErrorDto
from dtos.wallet import WalletDto
from services.walletService import WalletService

router = APIRouter(prefix="/wallet", tags=["wallet"])

@router.get("/", response_model=WalletDto, responses={404: {"model": ErrorDto}})
def get_wallet(userId: str, token: Annotated[str, Depends(oauth2_scheme)]):
    logger.info("Getting wallet with userId: " + userId, extra={"filetype": "walletController"})
    wallet = WalletService.get_wallet(userId)
    if(wallet == None):
        logger.info("Wallet with userId: " + userId + " not found", extra={"filetype": "walletController"})
        raise HTTPException(status_code=404, detail={
            "name": "Not Found", 
            "detail": "Wallet with userId: " + userId + " not found", 
            "ofType": "WalletDto", 
            "code": 404})
    logger.info("Wallet with userId: " + userId + " found", extra={"filetype": "walletController"})
    return wallet

@router.post("/", response_model=WalletDto)
def create_wallet(userId: str, token: Annotated[str, Depends(oauth2_scheme)]):
    logger.info("Creating wallet with userId: " + userId, extra={"filetype": "walletController"})
    wallet = WalletService.create_wallet(userId)

    logger.info("Wallet with userId: " + userId + " created successfully", extra={"filetype": "walletController"})
    return wallet

@router.put("/", response_model=WalletDto, responses={404: {"model": ErrorDto}})
def update_wallet(wallet: WalletDto, token: Annotated[str, Depends(oauth2_scheme)]):
    logger.info("Updating wallet with userId: " + wallet.userId, extra={"filetype": "walletController"})
    walletDto = WalletService.update_wallet(wallet)
    if(walletDto == None):
        logger.info("Wallet with userId: " + wallet.userId + " not found", extra={"filetype": "walletController"})
        raise HTTPException(status_code=404, detail={
            "name": "Not Found", 
            "detail": "Wallet with userId: " + wallet.userId + " not found", 
            "ofType": "WalletDto", 
            "code": 404})
    logger.info("Wallet with userId: " + wallet.userId + " updated successfully", extra={"filetype": "walletController"})
    return walletDto

@router.delete("/", responses={404: {"model": ErrorDto}, 201: {"model": SuccessDto}})
def delete_wallet(userId: str, token: Annotated[str, Depends(oauth2_scheme)]):
    logger.info("Deleting wallet with userId: " + userId, extra={"filetype": "walletController"})
    result = WalletService.delete_wallet(userId)
    if(result == None):
        logger.info("Wallet with userId: " + userId + " not found", extra={"filetype": "walletController"})
        raise HTTPException(status_code=404, detail={
            "name": "Not Found", 
            "detail": "Wallet with userId: " + userId + " not found", 
            "ofType": "WalletDto", 
            "code": 404})
    logger.info("Wallet with userId: " + userId + " deleted successfully", extra={"filetype": "walletController"})
    return HTTPException(status_code=201, detail={
        "detail": "Wallet with userId: " + userId + " deleted successfully"
    })