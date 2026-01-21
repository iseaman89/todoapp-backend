
# TodoApp Backend

This repository contains the backend API for the Todo application, built with **ASP.NET Core (.NET 8)** using a clean layered architecture.

## Features

- REST API
- JWT Authentication
- Entity Framework Core
- PostgreSQL
- Docker-ready

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL

## Run with Docker

The backend is designed to run via Docker Compose (see root README).

## API Endpoints

| Method | Endpoint | Description |
|------|---------|-------------|
| POST | /api/Auth/register | Register |
| POST | /api/Auth/login | Login |
| GET | /api/ToDo | Get todos |
| POST | /api/ToDo | Create todo |
| PUT | /api/ToDo | Update todo |
| GET | /api/ToDo{id} | Get todo |
| DELETE | /api/ToDo{id} | DELETE todo |
