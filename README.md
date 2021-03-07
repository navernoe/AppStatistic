# AppStatistic
Allows to load statistic information about application in Google Play Store.

This repo has 2 microservices:
1) http REST API with 2 endpoints:
 - GET /app?id={id} allows to get information from database about already exists application by its local uniq id
 - POST /app?URL={url} allows to add information about application in database by application url in Google Play Store
2) gRPC REST API Service that has one rpc method "getStatistic" for loading information about application from Google Play Store

---> Task Description: <---

Реализовать два микросервиса общающихся между собой посредством gRPC запросов. В качестве хранилища данных используется PostgreSQL. Весь сервис (оба микросервиса и база данных) должны подниматься одной командой через docker-compose.
Первый микросервис представляет из себя REST API. Существует всего два API Endpoint:

1) POST /app - принимает единственный параметр URL, адрес приложения в Google Play (например https://play.google.com/store/apps/details?id=com.instagram.android&hl=ru&gl=US), результатом выполнения запроса является уникальный ID приложения, который мы дали ему внутри нашей системы. Логика работы этого API - при вызове API создаётся запись о новом приложении в БД PostgreSQL (при этом нужно учесть ситуацию, когда приложение уже добавлено в базу, дублей быть не должно), и генерируется gRPC запрос к второму микросервису с требованием получить информацию по этому приложению с Google Play.

2) GET /app - принимает единственный параметр ID приложения, который мы дали ему вызвав предыдущий API. Логика работы этого API - по ID приложения из базы извлекается информация по этому приложению и возвращается в JSON ответе, либо 404, если ID приложения в базе не существует.
Второй микросервис слушает только gRPC запросы поступающие к нему извне. При поступлении запроса (запрос содержит ID приложения, по которому из БД PostgreSQL микросервис получает URL приложения) на обработку информации по приложению в Google Play, микросервис обращается к Google Play и получает следующую информацию по приложению: а) название приложения, б) количество скачиваний приложения. Полученные данные микросервис записывает по ID приложения в БД PostgreSQL.
