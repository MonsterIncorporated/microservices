import os

from fastapi.security import OAuth2AuthorizationCodeBearer

openIDAuthority = os.environ["OPENID_AUTHORITY"]

auth_url = f"{openIDAuthority}/protocol/openid-connect/auth"
token_url = f"{openIDAuthority}/protocol/openid-connect/token"

oauth2_scheme = OAuth2AuthorizationCodeBearer(
    authorizationUrl=auth_url,
    tokenUrl=token_url,
    scopes={},
)