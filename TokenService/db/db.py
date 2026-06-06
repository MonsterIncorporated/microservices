from sqlalchemy import create_engine
import os
import time
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
            print("Connected to the Database")
            return
        except Exception as e:
            print("Error connecting to the database, trying again in 3 seconds...", e)
            time.sleep(3)

connect_to_db()

