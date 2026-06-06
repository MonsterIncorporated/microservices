from fastapi import APIRouter, HTTPException
from logger import logger
from dtos.success import SuccessDto
from dtos.error import ErrorDto
from dtos.wallet import WalletDto
from services.walletService import WalletService

router = APIRouter(prefix="/wallet", tags=["wallet"])

@router.get("/", response_model=WalletDto, responses={404: {"model": ErrorDto}})
def get_wallet(userId: str):
    logger.info("Getting wallet with userId: " + userId)
    wallet = WalletService.get_wallet(userId)
    if(wallet == None):
        raise HTTPException(status_code=404, detail={
            "name": "Not Found", 
            "detail": "Wallet with userId: " + userId + " not found", 
            "ofType": "WalletDto", 
            "code": 404})
    return wallet

@router.post("/", response_model=WalletDto)
def create_wallet(userId: str):
    wallet = WalletService.create_wallet(userId)
    return wallet

@router.put("/", response_model=WalletDto, responses={404: {"model": ErrorDto}})
def update_wallet(wallet: WalletDto):
    walletDto = WalletService.update_wallet(wallet)
    if(walletDto == None):
        raise HTTPException(status_code=404, detail={
            "name": "Not Found", 
            "detail": "Wallet with userId: " + wallet.userId + " not found", 
            "ofType": "WalletDto", 
            "code": 404})
    return walletDto

@router.delete("/", responses={404: {"model": ErrorDto}, 201: {"model": SuccessDto}})
def delete_wallet(userId: str):
    result = WalletService.delete_wallet(userId)
    if(result == None):
        raise HTTPException(status_code=404, detail={
            "name": "Not Found", 
            "detail": "Wallet with userId: " + userId + " not found", 
            "ofType": "WalletDto", 
            "code": 404})
    return HTTPException(status_code=201, detail={
        "detail": "Wallet with userId: " + userId + " deleted successfully"
    })