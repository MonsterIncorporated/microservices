import logging
from loki_logging_handler.loki_handler import LokiHandler
from loki_logging_handler import JsonFormatter
import os

logger = logging.getLogger("custom_logger")
logger.setLevel(logging.DEBUG)

custom_handler = LokiHandler(
    url=os.environ["LOKI_URL"],
    labels={"app": "tokenservice-api"},
    formatter=JsonFormatter()
)
logger.addHandler(custom_handler)

logger.info("Logger Started for Token Service", extra={"filetype": "startup"})