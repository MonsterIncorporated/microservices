from pydantic import BaseModel


class ErrorDto(BaseModel):
    name: str
    detail: str
    ofType: str
    code: int

    model_config = {
        "from_attributes": True
    } 