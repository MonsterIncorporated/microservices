from enum import Enum
from sqlalchemy import String
from db.db import Base
from sqlalchemy.orm import Mapped, mapped_column

class TransactionStatus(str, Enum):
    PENDING = "PENDING"
    COMPLETED = "COMPLETED"

class Transaction(Base):
    __tablename__ = "transaction"

    transactionId: Mapped[str] = mapped_column(String(36), primary_key=True)
    userId: Mapped[str] = mapped_column(String(36))
    requiredTokens: Mapped[int]
    numberOfDigits: Mapped[int]
    status: Mapped[TransactionStatus] = mapped_column(String(20), default=TransactionStatus.PENDING.value)