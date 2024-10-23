# Chinook Superheroes

This repository contains a console application, superheroes database, and SQL scripts written in C#.

## SQL Commands and Database

### Database Overview

The superheroes database is designed to store information about various superheroes, their powers, and affiliations. It includes the following main tables:

- Superheroes
- Powers
- Teams
- Locations

### SQL Scripts

The `sql` directory contains the following SQL scripts:

1. `01_create_database.sql`: Creates the superheroes database.
2. `02_create_tables.sql`: Creates the database schema and tables.
3. `03_insert_sample_data.sql`: Populates the tables with sample data.
4. `04_stored_procedures.sql`: Contains stored procedures for common database operations.
5. `05_views.sql`: Defines views for frequently used queries.
6. `06_indexes.sql`: Creates indexes to optimize query performance.
7. `07_triggers.sql`: Implements triggers for maintaining data integrity and automating actions.
8. `08_functions.sql`: Defines user-defined functions for complex calculations or data manipulations.
9. `09_example_queries.sql`: Contains example queries for common operations and data retrieval.


### Usage

To set up the database:

1. Run `create_tables.sql` to create the database schema.
2. Execute `insert_data.sql` to populate the tables with sample data.
3. Use `queries.sql` as a reference for common database operations.

## Chinook Console Application

### Overview

The Chinook Console Application is a C# program that interacts with the Chinook database, allowing users to perform various operations such as querying, inserting, updating, and deleting superhero data.

### Features

- View all superheroes
- Search for a superhero by name
- Add a new superhero
- Update superhero information
- Delete a superhero
- View superhero powers and teams

### Getting Started

1. Ensure you have the .NET SDK installed on your machine.
2. Clone this repository.
3. Navigate to the `src` directory.
4. Run the application using the command: `dotnet run`

### Configuration

The application uses a `config.json` file to store database connection information. Make sure to update this file with your database credentials before running the application.

## Contributing

Please dont. 

## License

This project is licensed under the MIT License - see the LICENSE file for details.
