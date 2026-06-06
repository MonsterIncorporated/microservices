from pydantic import BaseModel


class SuccessDto(BaseModel):
    detail: str

    model_config = {
        "from_attributes": True
    } 