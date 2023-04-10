//https://learn.microsoft.com/en-us/dotnet/api/system.threading.thread?view=net-7.0
using System.Diagnostics;

namespace IlmaTorjunta;

class PosUpdater{
    List<Enemy>? enemiesCurrentPositions;
    string[]? mapCache;
    public PosUpdater(List<Enemy> _enemies, string[] _mapCache){
        enemiesCurrentPositions = _enemies;
        mapCache = _mapCache;
    }

    public void PosUpdaterMethod(){
        //Console.WriteLine("Venttas 2s");
        //enemiesCurrentPositions = enemies;
        //Thread.Sleep(2000);

        int movingTarget = 2;
        
        int newPosX = enemiesCurrentPositions.ElementAt(movingTarget).Pos.Item1;
        int newPosY = enemiesCurrentPositions.ElementAt(movingTarget).Pos.Item2;

        var sw = Stopwatch.StartNew();
        Console.WriteLine("Thread {0}: {1}, Priority {2}",
                            Thread.CurrentThread.ManagedThreadId,
                            Thread.CurrentThread.ThreadState,
                            Thread.CurrentThread.Priority);
        do {
            //Console.WriteLine("Thread {0}: Elapsed {1:N2} seconds",
            //                  Thread.CurrentThread.ManagedThreadId,
            //                  sw.ElapsedMilliseconds / 1000.0);
            if( !enemiesCurrentPositions.ElementAt(movingTarget).isDestroyed ){ //if not destroyed
                if( newPosX > 0 && newPosX < Program.mapSize.Item1 &&
                    newPosY > 0 && newPosY < Program.mapSize.Item2 ){
                    enemiesCurrentPositions.ElementAt(movingTarget).Pos = (newPosX++,newPosY);
                }else{
                    enemiesCurrentPositions.ElementAt(movingTarget).Pos = (newPosX--,newPosY);
                }
                EnemyFunctions.UpdateEnemyPos(enemiesCurrentPositions);
            }
            
            Thread.Sleep(1000);
        } while (sw.ElapsedMilliseconds <= 15000);
        sw.Stop();

        Console.WriteLine("Thread {0}: Done!", Thread.CurrentThread.ManagedThreadId);
    }

    public List<Enemy> PosUpdaterTrigger(){ //Tuple<int,int>[] enemies
        Console.WriteLine("Main (pos)thread: Start a second thread.");

        Thread t = new Thread(PosUpdaterMethod);
        t.Start();
        Console.WriteLine("Thread {0}: aloitettu", Thread.CurrentThread.ManagedThreadId);
        //t.Join();
        //Console.WriteLine("Main thread: ThreadProc.Join has returned.  Press Enter to end program.");

        return enemiesCurrentPositions;
    }
}
