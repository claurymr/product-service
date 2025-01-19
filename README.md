# Product Service

> **Warning:** This solution is part of the Inventory Management System and should be run together with the rest of the services. The whole system is composed of the following components:
> - [API Gateway](https://github.com/claurymr/store-api) <--- Complete setup instructions can be found here.
> - [Inventory Auth Service](https://github.com/claurymr/inventory-auth-service)
> - [Product Service](https://github.com/claurymr/product-service) (this repository)
> - [Inventory Service](https://github.com/claurymr/inventory-service)

## Overview
The Product Service is a RESTful API designed to manage product entries and their price history. It provides endpoints to create, read, update, and delete product data, as well as read product price history. 

This solution uses Docker for containerization, making it easy to run on macOS, Linux, and Windows.

## Development Approach Decisions

### Clean Architecture
To ensure the maintainability and scalability of the Product Service, I implemented Clean Architecture. This approach separates the application into distinct layers, promoting a clear separation of concerns and making the codebase easier to manage and extend.

### CQRS Pattern
I adopted the Command Query Responsibility Segregation (CQRS) pattern to provide better control over the steps when triggering events. This pattern separates read and write operations, allowing for more efficient and scalable handling of data.

### RabbitMQ as Message Broker
For message brokering, I chose RabbitMQ due to its easy setup and robust features. RabbitMQ facilitates communication between different parts of the system, ensuring reliable message delivery and processing. Moreover, no background services are required to be set up as RabbitMQ takes care of tracking consumers and publishers.

### FastEndpoints for Minimal APIs
I used FastEndpoints to set up minimal APIs in a structured and efficient manner. FastEndpoints aligns well with the CQRS pattern, providing a clean and organized way to define API endpoints.

### FluentValidation for Request Data Validation
To ensure the validity of incoming requests, I implemented FluentValidation. This library provides a fluent interface for defining validation rules, making it easy to enforce data integrity and provide meaningful error messages.

### Global Exception Handling
I implemented a simple global exception handling to manage errors consistently across the application and avoid raw exception messages to be returned in the response. This approach ensures that all exceptions are caught and handled gracefully, providing a better user experience and easier debugging.

### SQLite as the Database
For the database, I chose SQLite due to its lightweight nature and minimal setup requirements. For the nature of this project and to use time more sparringly I deemed other db setups unnecessary. SQLite is an embedded database that is easy to use and suitable for the needs of the Product Service, especially during development and testing phases.

### JWT Auth
To handle authentication and authorization, I implemented a simple JWT Auth by setting up predetermined credentials that don't need to be saved to a database. This decision was taken to simplify the process of authentication and authorization while prioritizing other functionalities, as registration and login were not required for the nature of this project. This approach allows for secure access control without the overhead of managing user credentials in a database.

## Forfeited Requirements

### Caching for GET Endpoints

I did not implement caching for the GET endpoints for the following reasons:
- **Low Priority:** The priority for implementing caching fell low compared to other critical features that needed to be developed and integrated into the system. Other functionalities and architectural decisions were deemed more pressing and required immediate attention to ensure the system's core capabilities were robust and reliable.

If caching had been implemented, Redis would have been the chosen solution. Redis is well-suited for distributed systems due to its high performance and scalability. However, at the time of this project, the system was not large enough to justify the additional complexity of integrating Redis. The focus was on delivering essential features and ensuring the system's stability before considering advanced optimizations like caching.