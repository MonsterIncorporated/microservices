from fastapi import FastAPI
from logger import logger
from controller.walletController import router as walletRouter
from controller.transactionController import router as transactionRouter
from db.db import Base, engine

logger.info("Starting Token Service")

Base.metadata.create_all(bind=engine)

api = FastAPI(docs_url="/")
api.title = "Token Service API"

api.include_router(walletRouter)
api.include_router(transactionRouter)

logger.info("Token Service started")