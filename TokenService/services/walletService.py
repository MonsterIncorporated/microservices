from dtos.wallet import WalletDto
from models.wallet import Wallet
from db.db import db

class WalletService:
    @staticmethod
    def get_wallet(userId: str):
        wallet = db.query(Wallet).filter(Wallet.userId == userId).first()
        if(wallet == None):
            return None
        return WalletDto.to_dto(wallet)
    
    @staticmethod
    def create_wallet(userId: str):
        wallet = Wallet(userId=userId, tokens=50)
        db.add(wallet)
        db.commit()
        return WalletDto.to_dto(wallet)
    
    @staticmethod
    def update_wallet(wallet: WalletDto):
        walletFromDb = db.query(Wallet).filter(Wallet.userId == wallet.userId).first()
        if walletFromDb == None:
            return None

        walletFromDb.tokens = wallet.tokens
        db.commit()

        return WalletDto.to_dto(walletFromDb)
    
    @staticmethod
    def delete_wallet(userId: str):
        walletFromDb = db.query(Wallet).filter(Wallet.userId == userId).first()
        if walletFromDb == None:
            return None

        db.delete(walletFromDb)
        db.commit()

        return True
            
