using System;
using System.Data;
using Npgsql;

public class DataAccessLayer
{
    NpgsqlConnection npgsqlcon;
    NpgsqlCommand npgsqlcmd;
    NpgsqlDataAdapter npgsqlAda;
    NpgsqlTransaction transact;
    public DataAccessLayer()
    {
        npgsqlcon = new NpgsqlConnection("server=localhost;port=14956;user id=healthmug_user;password=healthmug;database=postgres;command timeout=60");
    }
    public NpgsqlConnection NPGCON
    {
        get
        {
            return npgsqlcon;
        }
    }
    public DataTable GetDataTable(string query)
    {
        DataTable dt = new DataTable();
        try
        {
            npgsqlcmd = new NpgsqlCommand(query, npgsqlcon);
            npgsqlAda = new NpgsqlDataAdapter(npgsqlcmd);
            npgsqlAda.Fill(dt);
        }
        catch (Exception ex)
        {
            throw;
        }
        return dt;
    }
    public string ExecuteScaler(string query)
    {
        string str = "";
        try
        {
            npgsqlcon.Open();
            npgsqlcmd = new NpgsqlCommand(query, npgsqlcon);
            str = Convert.ToString(npgsqlcmd.ExecuteScalar());
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            npgsqlcon.Close();
        }
        return str;
    }
    public string ExecuteNonquery(string query)
    {
        string str = "";
        try
        {
            npgsqlcon.Open();
            npgsqlcmd = new NpgsqlCommand(query, npgsqlcon);
            str = Convert.ToString(npgsqlcmd.ExecuteNonQuery());
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            npgsqlcon.Close();
        }
        return str;
    }
    public string ExecuteTransaction(string query, string state = "false")
    {
        string str = "";
        bool donotcommit = false;
        if (state.ToLower() == "open" || state.ToLower() == "start")
        {
            npgsqlcon.Open();
            transact = npgsqlcon.BeginTransaction();
        }
        if (npgsqlcon.State == System.Data.ConnectionState.Closed)
        {
            npgsqlcon.Open();
            donotcommit = true;
        }
        try
        {
            npgsqlcmd = new NpgsqlCommand(query, npgsqlcon);
            str = Convert.ToString(npgsqlcmd.ExecuteScalar());
        }
        catch (Exception ex)
        {
            donotcommit = true;
            throw;
        }
        finally
        {
            if (state.ToLower() == "commit" || state.ToLower() == "close" || state.ToLower() == "end" || donotcommit == true)
            {
                if (donotcommit == false)
                    transact.Commit();
                npgsqlcon.Close();
            }

        }
        return str;
    }
}