import os

from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from services.rabbitMQService import RabbitMQService
from logger import logger
from controller.walletController import router as walletRouter
from controller.transactionController import router as transactionRouter
from db.db import Base, engine

logger.info("Starting Token Service", extra={"filetype": "startup"})

Base.metadata.create_all(bind=engine)

api = FastAPI(docs_url="/",swagger_ui_oauth2_redirect_url="/oauth2-redirect", swagger_ui_init_oauth={
    "clientId": "tokenservice-api-swagger",
    "usePkceWithAuthorizationCodeGrant": True
})

api.add_middleware(
    CORSMiddleware,
    allow_origins=[
        "http://localhost",
        "http://localhost:8082/transaction",
        "http://localhost:8082/transaction/"
        "http://localhost:8082/wallet"
    ],
    allow_methods=["*"],
    allow_headers=["*"]
)

api.title = "Token Service API"

api.include_router(walletRouter)
api.include_router(transactionRouter)

RabbitMQService.start_consuming_thread()

logger.info("Token Service started", extra={"filetype": "startup"})