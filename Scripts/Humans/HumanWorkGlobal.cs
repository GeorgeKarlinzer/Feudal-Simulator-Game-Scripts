using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static class HumanWorkGlobal
{
    private static List<WaitersValues> waiters = new List<WaitersValues>();

    public static void AddWaiter(string resName, int value, Worker worker)
    {
        if (waiters.Contains(new WaitersValues(resName, value, worker)))
            throw new System.Exception("Быдло ты ебаное нахуй!");

        waiters.Add(new WaitersValues(resName, value, worker));
    }

    public static void CheckWaiters(string resName, IStorage storage)
    {
        int count = waiters.Count;
        for (int i = 0; i < count; i++)
            if (waiters[i].resName == resName && GFSIS(storage, resName) >= waiters[i].value)
                if (!storage.IsLocal || storage == waiters[i].worker.workPlace)
                    if (MyAstarHandler.IsPathPossible(waiters[i].worker.transform, storage.Destination))
                    {
                        waiters[i].worker.workPlace.SendToStorage(waiters[i].worker);
                        waiters.Remove(waiters[i]);
                        i--;
                        count--;
                    }
    }

    /// <summary>
    /// Проверяет ожидающих рабочих асинхронно с задержкой (задержка нужна потому что изменения графа A* происходит асинхронно с задержкой)
    /// </summary>
    public async static void CheckWaitersDelayed(int delay = 500)
    {
        await Task.Run(() => Thread.Sleep(delay));
        int count = waiters.Count;
        for (int i = 0; i < count; i++)
        {
            string resName = waiters[i].resName;
            var storages = ResManager.res[resName].GetStorages();
            foreach (var s in storages)
                if (GFSIS(s, resName) >= waiters[i].value)
                    if (!s.IsLocal || s == waiters[i].worker.workPlace)
                        if (MyAstarHandler.IsPathPossible(s.Destination, waiters[i].worker.transform))
                        {
                            waiters[i].worker.workPlace.SendToStorage(waiters[i].worker);
                            waiters.Remove(waiters[i]);
                            i--;
                            count--;
                            break;
                        }
        }
    }

    /// <summary>
    /// Get free space in storage
    /// </summary>
    /// <param name="storage"></param>
    /// <param name="resName"></param>
    /// <returns></returns>
    private static int GFSIS(IStorage storage, string resName)
    {
        return storage.GetMaxAmount(resName) - storage.GetAmount(resName);
    }

    private struct WaitersValues
    {
        public WaitersValues(string resName, int value, Worker worker)
        {
            this.resName = resName;
            this.value = value;
            this.worker = worker;
        }

        public string resName;
        public int value;
        public Worker worker;
    }
}
