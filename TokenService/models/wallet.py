from sqlalchemy.orm import Mapped, mapped_column
from db import Base

class Wallet(Base):
    __tablename__ = "wallet"

    userId: Mapped[str] = mapped_column(primary_key=True)
    tokens: Mapped[int]