using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

class Donnees
{

    class Repertoire
    {
        Donnees _donnees = null;
        public Repertoire(Donnees donnees)
        {
            _donnees = donnees;
        }
        public void Add(string val)
        {
            _donnees.Add("REPERTOIRE", val);
        }
    }

    List<Repertoire> _repertoire;
    public Donnees()
    {
        _repertoire = new List<Repertoire>();
    }

    private object _lock = new object();

    public string _connectionString;
    public virtual string ConnectionString
    {
        set { _connectionString = value; }
        get { return _connectionString; }
    }

    public void InitializeDatabase(string connectionString)
    {
        Debug.AssertStringNotEmpty(connectionString);
        ConnectionString = connectionString;

        string dbFilePath = connectionString;

        if (File.Exists(dbFilePath))
            return;

        //
        // Make sure that we don't have multiple threads all trying to create the database
        //

        lock (_lock)
        {
            //
            // Just double check that no other thread has created the database while
            // we were waiting for the lock
            //

            if (File.Exists(dbFilePath))
                return;

            SQLiteConnection.CreateFile(dbFilePath);

            const string sql = @"
                CREATE TABLE Fichier (
                    Idt INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    FullName TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    Size INTEGER NOT NULL,
                    DteMdf TEXT NOT NULL,
                    hashCode TEXT NOT NULL
                )";

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + connectionString))
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }

    internal void Add(string prmNom, string val)
    {
        //
        // Just double check that no other thread has created the database while
        // we were waiting for the lock
        //

        const string sql = @"INSERT INTO PrmVal( Prm_Idt, Seq_Num, Val )
select p.Idt, ifnull(max(pv.Seq_Num), 0)+1, '{0}'
  from Prm p
  left join PrmVal pv on pv.Prm_Idt = p.Idt
 where p.Nom = '{1}'";
        string req = String.Format(sql, val, prmNom);
        ExecuteSQL(req);
    }

    private void ExecuteSQL(string req)
    {
        lock (_lock)
        {

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + ConnectionString))
            using (SQLiteCommand command = new SQLiteCommand(req, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }

    public Boolean GetRow(FileInfo pFile)
    {
        Boolean found = false;
        lock (_lock)
        {
            const string sql = @"
                select count(*) NBR from Fichier
                where FullName = '{0}'";

            string req = string.Format(sql, pFile.FullName.Replace("'", "''"));

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + ConnectionString))
            using (SQLiteCommand command = new SQLiteCommand(req, connection))
            {
                connection.Open();
                SQLiteDataReader rdr = command.ExecuteReader();

                while (rdr.Read())
                {
                    if (Convert.ToInt32(rdr["NBR"]) == 1)
                    {
                        found = true;
                    }
                }
            }
        }
        return found;
    }

    public void InsertRow(FileInfo pFile, string pHashCode)
    {

        lock (_lock)
        {
            //
            // Just double check that no other thread has created the database while
            // we were waiting for the lock
            //

            const string sql = @"
                INSERT INTO Fichier
                ( FullName, Name, Size, DteMdf, hashCode
                ) VALUES
                ( '{0}', '{1}', {2}, '{3}', '{4}'
                )";

            string req = string.Format(sql, pFile.FullName.Replace("'", "''"), pFile.Name.Replace("'", "''"), pFile.Length, pFile.LastWriteTimeUtc.ToString(), pHashCode);

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + ConnectionString))
            using (SQLiteCommand command = new SQLiteCommand(req, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}