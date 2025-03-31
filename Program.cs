using System;
using System.Collections.Generic;
using Dapper;

partial class Program
{
    static void Main(string[] args)
    {
        string choice;

        do
        {
            Console.WriteLine("\n=== Game Record Keeping Application ===");
            Console.WriteLine("1. Manage Teams");
            Console.WriteLine("2. Manage Seasons");
            Console.WriteLine("3. Create a New Game");
            Console.WriteLine("4. Display All Game Records");
            Console.WriteLine("5. Display Team Game History");
            Console.WriteLine("6. Exit");
            Console.Write("Choose an option: ");
            choice = Console.ReadLine();

            if (choice == "1")
            {
                ManageTeams();
            }
            else if (choice == "2")
            {
                ManageSeasons();
            }
            else if (choice == "3")
            {
                CreateNewGame();
            }
            else if (choice == "4")
            {
                DisplayAllGameRecords();
            }
            else if (choice == "5")
            {
                DisplayTeamGameHistory();
            }
            else if (choice == "6")
            {
                Console.WriteLine("Exiting the program. Goodbye!");
                Console.WriteLine("Programmer : Mark Joseph Potot");
            }
            else
            {
                Console.WriteLine("Di mao imo gi input, balik kana saken");
                return;
            }
        } while (choice != "6");
    }

    static void ManageTeams()
    {
        string teamChoice;
        do
        {
            Console.WriteLine("\n--- Manage Teams ---");
            Console.WriteLine("1. Add a New Team");
            Console.WriteLine("2. Display All Teams");
            Console.WriteLine("3. Return to Main Menu");
            Console.Write("Choose an option: ");
            teamChoice = Console.ReadLine();

            if (teamChoice == "1")
            {
                AddNewTeam();
            }
            else if (teamChoice == "2")
            {
                DisplayAllTeams();
            }
            else if (teamChoice == "3")
            {
                // Return to main menu
            }
            else
            {
                Console.WriteLine("Invalid option, please try again.");
            }
        } while (teamChoice != "3");
    }

    static void ManageSeasons()
    {
        string seasonChoice;
        do
        {
            Console.WriteLine("\n--- Manage Seasons ---");
            Console.WriteLine("1. Add a New Season");
            Console.WriteLine("2. Display All Seasons");
            Console.WriteLine("3. Return to Main Menu");
            Console.Write("Choose an option: ");
            seasonChoice = Console.ReadLine();

            if (seasonChoice == "1")
            {
                AddNewSeason();
            }
            else if (seasonChoice == "2")
            {
                DisplayAllSeasons();
            }
            else if (seasonChoice == "3")
            {
                // Return to main menu
            }
            else
            {
                Console.WriteLine("Invalid option, please try again.");
            }
        } while (seasonChoice != "3");
    }

    static void AddNewTeam()
    {
        Console.Write("Enter Team Name: ");
        string teamName = Console.ReadLine();
        Console.Write("Enter Team Country (optional): ");
        string teamCountry = Console.ReadLine();

        var connection = DatabaseConnection.GetDatabaseConnection();


        var query = @"INSERT INTO Teams (TeamName, Country) 
                VALUES (@teamName, @teamCountry);
                SELECT * FROM Teams WHERE TeamId = LAST_INSERT_ID();";


        var newTeam = connection.QuerySingle<Team>(query, new
        {
            teamName,
            teamCountry = string.IsNullOrEmpty(teamCountry) ? null : teamCountry
        });

        Console.WriteLine($"New team created with ID: {newTeam.TeamId}");
    }
    static void DisplayAllTeams()
    {
        var connection = DatabaseConnection.GetDatabaseConnection();
        var teams = connection.Query<Team>("SELECT * FROM Teams");

        Console.WriteLine("\n--- Teams ---");
        foreach (var team in teams)
        {
            Console.WriteLine($"ID: {team.TeamId} | Name: {team.TeamName} | Country: {team.Country ?? "N/A"}");
        }
    }

    static void AddNewSeason()
    {
        Console.Write("Enter Season Name: ");
        string seasonName = Console.ReadLine();
        Console.Write("Enter Season Description: ");
        string description = Console.ReadLine();

        var connection = DatabaseConnection.GetDatabaseConnection();


        var query = @"INSERT INTO Seasons (SeasonName, Description) 
                VALUES (@seasonName, @description);
                SELECT * FROM Seasons WHERE SeasonId = LAST_INSERT_ID();";


        var newSeason = connection.QuerySingle<Season>(query, new
        {
            seasonName,
            description
        });

        Console.WriteLine($"New season created with ID: {newSeason.SeasonId}");
    }

    static void DisplayAllSeasons()
    {
        var connection = DatabaseConnection.GetDatabaseConnection();
        var seasons = connection.Query<Season>("SELECT * FROM Seasons");

        Console.WriteLine("\n--- Seasons ---");
        foreach (var season in seasons)
        {
            Console.WriteLine($"ID: {season.SeasonId} | Name: {season.SeasonName} | Description: {season.Description}");
        }
    }

    static void CreateNewGame()
    {
        Console.WriteLine("\n--- Create New Game ---");
        Console.WriteLine("Available Seasons:");
        DisplayAllSeasons();

        Console.Write("\nEnter Season ID: ");
        int seasonId = int.Parse(Console.ReadLine());

        Console.WriteLine("Available Teams:");
        DisplayAllTeams();

        Console.Write("Enter Team A ID: ");
        int teamAId = int.Parse(Console.ReadLine());
        Console.Write("Enter Team B ID: ");
        int teamBId = int.Parse(Console.ReadLine());
        Console.Write("Enter starting score for Team A: ");
        int scoreA = int.Parse(Console.ReadLine());
        Console.Write("Enter starting score for Team B: ");
        int scoreB = int.Parse(Console.ReadLine());

        var connection = DatabaseConnection.GetDatabaseConnection();
        var query = @"INSERT INTO Games (SeasonId, TeamAId, TeamBId, ScoreA, ScoreB, StartTime) 
                VALUES (@seasonId, @teamAId, @teamBId, @scoreA, @scoreB, NOW());
                SELECT g.GameId, s.SeasonName, t1.TeamName AS TeamAName, 
                       t2.TeamName AS TeamBName, g.ScoreA, g.ScoreB, g.StartTime
                FROM Games g
                JOIN Seasons s ON g.SeasonId = s.SeasonId
                JOIN Teams t1 ON g.TeamAId = t1.TeamId
                JOIN Teams t2 ON g.TeamBId = t2.TeamId
                WHERE g.GameId = LAST_INSERT_ID();";


        var newGame = connection.QuerySingle<GameRecord>(query, new
        {
            seasonId,
            teamAId,
            teamBId,
            scoreA,
            scoreB
        });

        Console.WriteLine($"New game created with ID: {newGame.GameId}");
    }

    static void DisplayAllGameRecords()
    {
        var connection = DatabaseConnection.GetDatabaseConnection();
        var query = @"
            SELECT g.GameId, s.SeasonName, t1.TeamName AS TeamAName, t2.TeamName AS TeamBName, 
                   g.ScoreA, g.ScoreB, g.StartTime
            FROM Games g
            JOIN Seasons s ON g.SeasonId = s.SeasonId
            JOIN Teams t1 ON g.TeamAId = t1.TeamId
            JOIN Teams t2 ON g.TeamBId = t2.TeamId";

        var games = connection.Query<GameRecord>(query);

        Console.WriteLine("\n--- Game Records ---\n");

        foreach (var game in games)
        {
            if (game.ScoreA > game.ScoreB)
            {
                Console.WriteLine($"Game ID: {game.GameId} | Season: {game.SeasonName} | {game.TeamAName} vs {game.TeamBName} | Score: {game.ScoreA}-{game.ScoreB} | Winner: {game.TeamAName} | Started: {game.StartTime}");
            }
            else if (game.ScoreB > game.ScoreA)
            {
                Console.WriteLine($"Game ID: {game.GameId} | Season: {game.SeasonName} | {game.TeamAName} vs {game.TeamBName} | Score: {game.ScoreA}-{game.ScoreB} | Winner: {game.TeamBName} | Started: {game.StartTime}");
            }
            else
            {
                Console.WriteLine($"Game ID: {game.GameId} | Season: {game.SeasonName} | {game.TeamAName} vs {game.TeamBName} | Score: {game.ScoreA}-{game.ScoreB} | Draw | Started: {game.StartTime}");
            }
        }
    }

    static void DisplayTeamGameHistory()
    {
        Console.WriteLine("\n--- Teams ---");
        DisplayAllTeams();

        Console.Write("\nEnter Team ID to view game history: ");
        int teamId = int.Parse(Console.ReadLine());

        var connection = DatabaseConnection.GetDatabaseConnection();
        var query = @"
            SELECT g.GameId, s.SeasonName, t1.TeamName AS TeamAName, t2.TeamName AS TeamBName, 
                   g.ScoreA, g.ScoreB, g.StartTime
            FROM Games g
            JOIN Seasons s ON g.SeasonId = s.SeasonId
            JOIN Teams t1 ON g.TeamAId = t1.TeamId
            JOIN Teams t2 ON g.TeamBId = t2.TeamId
            WHERE t1.TeamId = @teamId OR t2.TeamId = @teamId";

        var games = connection.Query<GameRecord>(query, new { teamId });

        Console.WriteLine("\n--- Team Game History ---\n");
        foreach (var game in games)
        {
            Console.WriteLine($"Game ID: {game.GameId} | Season: {game.SeasonName} | {game.TeamAName} vs {game.TeamBName} | Score: {game.ScoreA}-{game.ScoreB} | Started: {game.StartTime}");
        }
    }
}