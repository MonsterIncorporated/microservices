import logging
from loki_logging_handler.loki_handler import LokiHandler
import os

logger = logging.getLogger("custom_logger")
logger.setLevel(logging.DEBUG)

custom_handler = LokiHandler(
    url=os.environ["LOKI_URL"],
    labels={"app": "tokenservice-api"},
)
logger.addHandler(custom_handler)