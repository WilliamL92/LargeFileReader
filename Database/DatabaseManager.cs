using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace Database
{
    public class DatabaseManager
    {
        private readonly string _connectionString;
        private readonly string _databasePath;

        public DatabaseManager(string databaseName = "localdb.sqlite")
        {
            // Définir le chemin de la base de données dans le dossier de l'application
            _databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, databaseName);
            _connectionString = $"Data Source={_databasePath}";

            // Initialiser la base de données si elle n'existe pas
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            // Créer le fichier de base de données s'il n'existe pas
            if (!File.Exists(_databasePath))
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    // Créer quelques tables d'exemple
                    using var command = connection.CreateCommand();
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Users (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Username TEXT NOT NULL,
                            Email TEXT,
                            CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP
                        );

                        CREATE TABLE IF NOT EXISTS Notes (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            UserId INTEGER,
                            Title TEXT NOT NULL,
                            Content TEXT,
                            CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP,
                            FOREIGN KEY (UserId) REFERENCES Users(Id)
                        );
                    ";
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Exécuter une requête SELECT et retourner les résultats
        /// </summary>
        public List<Dictionary<string, object>> ExecuteQuery(string sql, Dictionary<string, object>? parameters = null)
        {
            var results = new List<Dictionary<string, object>>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = sql;

                // Ajouter les paramètres s'ils existent
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.GetValue(i);
                    }
                    results.Add(row);
                }
            }

            return results;
        }

        /// <summary>
        /// Exécuter une commande non-query (INSERT, UPDATE, DELETE)
        /// </summary>
        public int ExecuteNonQuery(string sql, Dictionary<string, object>? parameters = null)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            // Ajouter les paramètres s'ils existent
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// Exemple: Ajouter un utilisateur
        /// </summary>
        public long AddUser(string username, string? email = null)
        {
            var sql = "INSERT INTO Users (Username, Email) VALUES (@username, @email); SELECT last_insert_rowid();";
            var parameters = new Dictionary<string, object>
            {
                { "@username", username },
                { "@email", email ?? string.Empty }
            };

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            return (long)command.ExecuteScalar();
        }

        /// <summary>
        /// Exemple: Récupérer tous les utilisateurs
        /// </summary>
        public List<Dictionary<string, object>> GetAllUsers()
        {
            return ExecuteQuery("SELECT * FROM Users");
        }

        /// <summary>
        /// Obtenir le chemin de la base de données
        /// </summary>
        public string GetDatabasePath()
        {
            return _databasePath;
        }
    }
}
