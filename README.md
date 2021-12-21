# AuthenticationService

Zorg ervoor dat er een rabbitmq-management docker container draait met deze command:
```
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.9-management
```

Run het commando `docker-compose up` voor development in de main map.

Daarna heb je toegang tot de swagger pagina met deze url: http://localhost:5000/swagger/index.html.

## Swagger endpoints

### /api/Auth

Dit endpoint is gemaakt om te testen of de service reageerd. 

Code 200 geeft een string terug "Wrm werkt nooit iets gelijk in 1 keer?!".

### /api/Auth/singin

Dit endpoint is het login endpoint. De body van dit request moet een email en een wachtwoord bevatten als type string. De body moet zijn geschreven is JSON.

Code 200 geeft een JSON body terug. Hierin staat een Id en een JWT token.

Code 400 geeft bad request terug. In de message staat dat er een invalid username/password bevat.

### /api/Auth/register

Dit endpoint is het registreer endpoint. De body van dit request moet een voornaam, achternaam, email en wachtwoord bevatten als type string. De body moet zijn geschreven in JSON.

Code 200 heeft een JSON body terug. Hierin staat een Id en een JWT token.

Code 400 geeft een bad request terug. Dit betekend dat de rabbitmq management niet (goed) draait.
