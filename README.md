<p align="center">
<a href="https://github.com/ErrorNotFound/ImbaBet/actions"><img src="https://github.com/ErrorNotFound/ImbaBet/actions/workflows/build-docker.yaml/badge.svg"></a>
<a href="https://hub.docker.com/r/1337festor/imbabetweb"><img src="https://img.shields.io/badge/ImbaBet-DockerHub-blue"></a>
<a href="https://github.com/ErrorNotFound/ImbaBet/releases"><img src="https://img.shields.io/github/v/release/ErrorNotFound/ImbaBet"></a>
<img src="/images/logo.png" width="80%">


</p>

# Welcome to my project

ImbaBet is a self-hostable betting game, that lets you and your friends compete against each other for the next major sporting event (e.g. a football World Cup).

- [Features](#features)
- [Getting started](#getting-started)
- [References](#references)

<img src="/images/screenshot.png" width="70%">

# Features
- Simple, but appealing UI
- User, community and community-internal leaderboards
- Join forces with friends to form a community and compete with others
- Easy administration for your betting game

# Getting started
The easiest way to deploy ImbaBet is to use the docker-compose and .env in the [`/docker`](https://github.com/ErrorNotFound/ImbaBet/tree/master/docker) directory.
After starting your container, you can open http://<SERVER_ADDRESS>:\<PORT>/admin/createadmin to generate an initial admin account with the following credentials:

## Default admin credentials
- **E-Mail**: `admin@admin.de`
- **Password**: `Admin1!`

## Environment variables
| ENV name				| Description									|
|-----------------------|-----------------------------------------------|
|DATABASE_PASSWORD		| Password used for sql database, see <a href="https://hub.docker.com/r/microsoft/mssql-server">Microsoft SQL Server Image</a>.| 
|ADMIN_ACCOUNT_EMAIL	| Overwrites the default admin account email	|
|ADMIN_ACCOUNT_USERNAME	| Overwrites the default admin account username	|
|ADMIN_ACCOUNT_PASSWORD	| Overwrites the default admin account password	|

# References
- Country flags from https://flagpedia.net/
- FluentValidation (https://github.com/FluentValidation/FluentValidation)