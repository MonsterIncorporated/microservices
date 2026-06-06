from pydantic import BaseModel
from models.wallet import Wallet

class WalletDto(BaseModel):
    userId: str
    tokens: int

    model_config = {
        "from_attributes": True
    }

    @staticmethod
    def to_dto(wallet: Wallet):
        return WalletDto(userId=wallet.userId, tokens=wallet.tokens)
    
    @staticmethod
    def to_entity(walletDto: WalletDto):
        return Wallet(userId=walletDto.userId, tokens=walletDto.tokens)