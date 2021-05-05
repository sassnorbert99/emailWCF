using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Data;

class SQLiteHelper
{
    private SQLiteHelper() { }
    static SQLiteHelper instance;
    public static SQLiteHelper GetSQLiteHelper()
    {
        if (instance == null) instance = new SQLiteHelper();
        return instance;
    }

    SQLiteDataAdapter dataAdapter;
    DataSet dSet = new DataSet();
    DataTable dTable = new DataTable();
    SQLiteCommand cmd;
    SQLiteConnection sqliteCon;
    string filesource;
    public string FileSource
    {
        get { return filesource; }
        set { filesource = value; }
    }


    public void NewFile(string DataBaseFileSource)
    {
        if (sqliteCon != null) sqliteCon.Close();
        this.FileSource = DataBaseFileSource;
        SetConnection(this.FileSource);
        CreateFile(this.FileSource);
    }
    public void OpenFile(string DataBaseFileSource)
    {
        if (sqliteCon != null) sqliteCon.Close();
        this.FileSource = DataBaseFileSource;
        SetConnection(this.FileSource);
    }

    private void CreateFile(string filesource)
    {
        try
        {
            if (File.Exists(filesource))
            {
                File.Delete(filesource);
                System.Threading.Thread.Sleep(2000);
            }
            SQLiteConnection.CreateFile(filesource);
        }
        catch { }
    }



    public void Connect()
    {
        if (sqliteCon != null) sqliteCon.Open();
    }

    public bool IsOpen
    {
        get
        {
            if (sqliteCon == null) return false;
            return (System.Data.ConnectionState.Open == sqliteCon.State);
        }
    }

    public void Open()
    {
        sqliteCon.Open();
    }

    public void Close()
    {
        sqliteCon.Close();
    }

    public void SetConnection(string filesource)
    {
        sqliteCon = new SQLiteConnection("Data Source=" + filesource + ";Version=3;");
    }

    #region SQLite IO Operations

    /// <summary>
    /// Executes a single non query command
    /// </summary>
    public void ExecuteNonQuery(string commandSQL)
    {
        Open();
        cmd = new SQLiteCommand(commandSQL, sqliteCon);
        cmd.ExecuteNonQuery();
        Close();
    }

    /// <summary>
    /// Executes a query command which returns one single value
    /// </summary>
    public string ExecuteOneReader(string commandSQL, string returnColumn)
    {
        Open();
        cmd = new SQLiteCommand(commandSQL, sqliteCon);
        SQLiteDataReader reader = cmd.ExecuteReader();
        reader.Read();
        Close();
        return reader[returnColumn].ToString();
    }

    /// <summary>
    /// Executes a query command which returns a column of values
    /// </summary>
    public List<string> ExecuteMoreReader(string commandSQL, string returnColumn)
    {
        Open();
        List<string> items = new List<string>();
        cmd = new SQLiteCommand(commandSQL, sqliteCon);
        SQLiteDataReader reader = cmd.ExecuteReader();
        while (reader.Read()) items.Add((string)reader[returnColumn]);
        Close();
        return items;
    }

    /// <summary>
    /// Executes a query command which returns a table of values.
    /// This can be given to a WPF Grid, as DataSource, or can be shown by
    /// the ShowDataResult method.
    /// </summary>
    public DataTable ExecuteDataQuery(string CommandSQL)
    {
        Open();
        cmd = sqliteCon.CreateCommand();
        dataAdapter = new SQLiteDataAdapter(CommandSQL, sqliteCon);
        dSet.Reset();
        dataAdapter.Fill(dSet);
        dTable = dSet.Tables[0];
        Close();
        return dTable;
    }

    #endregion

    public void ShowDataResult(DataTable Result, bool ShowHeader)
    {
        if (ShowHeader)
        {
            for (int i = 0; i < Result.Columns.Count; i++)
            {
                Console.Write("{0} ", Result.Columns[i].ColumnName);
            }
            Console.WriteLine();
        }
        foreach (DataRow row in Result.Rows)
        {
            for (int i = 0; i < Result.Columns.Count; i++)
            {
                Console.Write("{0} ", row[Result.Columns[i].ColumnName]);
            }
            Console.WriteLine();
        }
    }

    ~SQLiteHelper()
    {
        try { Close(); }
        catch { }
    }

}
