from sqlalchemy import String
from sqlalchemy.orm import Mapped, mapped_column
from db.db import Base

class Wallet(Base):
    __tablename__ = "wallet"

    userId: Mapped[str] = mapped_column(String(36), primary_key=True)
    tokens: Mapped[int]