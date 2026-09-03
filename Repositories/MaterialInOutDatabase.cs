using material_inout_desktop_v2.Entities;
using SQLite;

namespace material_inout_desktop_v2.Repositories;

public class MaterialInOutDatabase : IMaterialInOutDatabase
{
    protected SQLiteAsyncConnection? Database;
    /// <summary>
    /// We need to lock the initialization not that we get a database back with only half the tables intialized
    /// </summary>
    private readonly SemaphoreSlim _initializationLock = new SemaphoreSlim(1, 1);

    private async Task Init()
    {
        await _initializationLock.WaitAsync();
        try
        {
            if (Database != null)
            {
                return;
            }
            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

            await Database.CreateTableAsync<Article>();
            
        }
        finally
        {
            _initializationLock.Release();
        }
    }
}