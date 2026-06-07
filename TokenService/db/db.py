from sqlalchemy import create_engine
import os
import time
from logger import logger
from sqlalchemy.orm import sessionmaker, DeclarativeBase

DATABASE_URL = "mysql+pymysql://" + os.environ['MYSQL_USER'] + ":" + os.environ['MYSQL_PASS'] + "@" + os.environ['MYSQL_HOST'] + "/tokens"
engine = None
sessionLocal = None
db = None

class Base(DeclarativeBase):
    pass

def connect_to_db():
    global engine, sessionLocal, db
    while True:
        try:
            engine = create_engine(DATABASE_URL)
            engine.connect()
            sessionLocal = sessionmaker(bind=engine, autoflush=False, autocommit=False)
            db = sessionLocal()
            logger.info("Connected to the database successfully", extra={"type": "startup"})
            return
        except Exception as e:
            logger.error("Error connecting to the database: " + str(e), extra={"type": "startup"})
            logger.info("Retrying in 3 seconds...", extra={"type": "startup"})
            time.sleep(3)

connect_to_db()

