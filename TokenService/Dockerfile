FROM python:3.14.5-alpine3.23
WORKDIR /usr/src/app

RUN pip install pipenv

COPY Pipfile Pipfile.lock ./
RUN pipenv install

COPY . ./

CMD ["pipenv", "run", "fastapi", "run", "main.py", "--host", "0.0.0.0", "--port", "8080"]