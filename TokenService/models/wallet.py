from sqlalchemy.orm import Mapped, mapped_column
from database import Base

class Wallet(Base):
    __tablename__ = "wallet"

    userId: Mapped[str](primary_key=True)
    tokens: Mapped[int]