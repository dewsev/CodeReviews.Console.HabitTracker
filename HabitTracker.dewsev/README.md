# Habit Tracker

Console based CRUD application to track habit occurrences.

Technologies: C#, SQL (SQLite)

## Given Requirements:

- [x] Users need to be able to input the date of the occurrence of the habit
- [x] The application should store and retrieve data from a real database
- [x] When the application starts, it should create a sqlite database, if one isn’t present
- [x] It should also create a table in the database, where the habit will be logged
- [x] Seed Data into the database automatically when the database gets created for the first time
- [x] Let the users create their own habits to track. That will require that you let them choose the unit of measurement of each habit.
- [x] You should handle all possible errors so that the application never crashes
- [x] You can only interact with the database using ADO.NET. You can’t use mappers such as Entity Framework or Dapper
- [x] Try using parameterized queries to make your application more secure.
- [x] You should handle all possible errors so that the application never crashes
- [x] Follow the DRY Principle, and avoid code repetition.

## Features

* SQLite database connection
	- The program uses a SQLite db connection to store and read information
	- If no database exists, the required tables are created and seed data is inserted
* A console based UI
* CRUD DB functions
	- From the main menu users can Create, Read, Update or Delete habits and occurrences
	- Dates inputted are checked to make sure they are in the correct and realistic format (dd/MM/yyyy). 
* Basic Reports of habit occurrences

## Challenges
	
- It was my first time using SQLite. I had to learn it from the ground up in order to complete this project.
- I had to learn how to create and execute SQL commands using ADO.NET
- I needed to insert seed data, which was the first time I was doing it. I had to think about how to approach it and how to check if the data was already seeded or not
- I had to learn about parameterized queries that prevent SQL Injection and improve performance

## Areas to Improve
- KISS principle - I had to refactor my code multiple times due to the fact it was starting to become a bit unreadable. I need to practice it more, and try not to over-engineer it at the same time

## Resources Used
- [C# project setup with SQLite and ADO.NET](https://www.youtube.com/watch?v=d1JIJdDVFjs&feature=youtu.be)
- [C# KISS Principle (Keep It Simple, Stupid!](https://bytehide.com/blog/kiss-principle-csharp)
- [Mastering Parameterized Queries in ADO.NET](https://reintech.io/blog/mastering-parameterized-queries-ado-net)