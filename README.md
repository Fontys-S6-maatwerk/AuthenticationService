# AuthenticationService

Zorg ervoor dat er een rabbitmq-management docker container draait met deze command:
```
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.9-management
```

Run het commando `docker-compose up` voor development in de main map.

Daarna heb je toegang tot de swagger pagina met deze url: http://localhost:5000/swagger/index.html.

