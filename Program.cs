
namespace IlmaTorjunta;
/*
Keys
UpArrow
RightArrow
DownArrow
LeftArrow
Enter

Console.WriteLine(Console.ReadKey(true).Key);
*/

class Enemy{
    static string[] types = new string[] {  
        "fighter", 
        "bomber", 
        "plane",
        "civilian-helicopter", 
        "military-helicopter", 
        "attack-helicopter",
        "unknown"
    };
    public int Type {get;set;}
    public string typeAsString;
    public string CallSign {get;set;}

    public (int x, int y) Pos {get;set;}

    public bool isTargeted;
    public bool isDestroyed; 

    public Enemy(int _type, string _callSign, (int x, int y) _pos ){
        Type = _type;
        typeAsString = types[Type];
        CallSign = _callSign;
        Pos = _pos;
    }
}

class EnemyFunctions
{

    public static List<Enemy> enemyList = new List<Enemy>() {
        new Enemy(1,"foo1", (15,3)),
        new Enemy(1,"foo2", (4,4)),
        new Enemy(1,"foo3", (2,2)),
    };

    //start values
    static public Tuple<int,int>[] enemies = new Tuple<int, int>[] { 
        new Tuple<int,int>(15,3),
        new Tuple<int,int>(4,4),
        new Tuple<int,int>(2,2),
    };
    static public bool[] lockedEnemies = new bool[] { false,false,false };


    static public bool AttackEnemy(int enemyIndex){
        if (OperatingSystem.IsWindows()){
            Console.Beep(500,800);
        }
        //enemies[enemyIndex] = new Tuple<int,int>(-1,-1);
        EnemyFunctions.enemyList.ElementAt(enemyIndex).isDestroyed = true;

        Console.WriteLine("-Destroyed-");
        return true;
    }

    public static List<Enemy> EnemyPosUpdater(string[] _mapCache){
        Console.WriteLine("Enemy pos update started");
        PosUpdater pu = new PosUpdater(EnemyFunctions.enemyList, _mapCache);
        return pu.PosUpdaterTrigger();
    }

    public static void UpdateEnemyPos(List<Enemy> _enemies){
        enemyList = _enemies;
        //makeMap(char[] charList, (int,int) size){
        
        Program.UpdateMap( Program.makeMap(Program.mapChars, Program.mapSize) );
    }

    public static char DetermineChar(int enemyIndex){
        char enemyChar = Program.mapChars[2];
        if( EnemyFunctions.enemyList.ElementAt(enemyIndex).isTargeted ) enemyChar = Program.mapChars[5];
        return enemyChar;
    }
}

class Position{
    public Position(int x, int y){
        PosX = x;
        PosY = y;
    }
    public int PosX {get;set;}
    public int PosY {get;set;}

    public bool targetLocked;

    public int CheckIfEnemy(){
        int targetOk = -1;
        int index = -1;
        foreach (var oneEnemy in EnemyFunctions.enemyList)
        {
            index++;
            if( (oneEnemy.Pos.Item1 == PosX && oneEnemy.Pos.Item2 == PosY) || oneEnemy.isTargeted ){
                targetOk = index;
            }
        }
        return targetOk;
    }
}

class Program
{
    public static string[]? mapCache;
    public static char[] mapChars = new char[] {
        '#',    //0 border
        ' ',    //1 space
        '¤',    //2 enemy
        '+',    //3 cursor
        '*',    //4 attack
        'X'     //5 lock
    };

    public static (int,int) mapSize = (20,10);

    static string[] moveCharacter(string[] map, Position pos, char charToUpdate){
        if(pos.PosY >=0 && pos.PosX >=0){
            string yRow = map[pos.PosY]; //pos y
            char[] chars = yRow.ToCharArray();
            chars[pos.PosX] = charToUpdate; //pos x
            map[pos.PosY] = new string(chars);
        }else{
            Console.WriteLine("Out of bounds!");
        }
        return map;
    }

    public static string[] makeMap(char[] charList, (int,int) size){
        string[] map = new string[size.Item2];
        for (int i = 0; i < size.Item2; i++)
        {
            if(i == 0 || i == size.Item2-1){
                string border = new String(charList[0], size.Item1);
                map[i] = charList[0] + border + charList[0];
            }else{
                string space = new String(charList[1], size.Item1);
                int enemyIndex = 0;
                foreach (var oneEnemy in EnemyFunctions.enemyList)
                {
                    if(i == oneEnemy.Pos.Item2 && !oneEnemy.isDestroyed){ //pos y
                        char[] chars = space.ToCharArray();
                        chars[oneEnemy.Pos.Item1-1] = EnemyFunctions.DetermineChar(enemyIndex); //pos x
                        space = new string(chars);
                    }     
                    enemyIndex++;          
                }

                map[i] = charList[0] + space + charList[0];
            }
        }
        mapCache = map;
        return map;
    }

    static string[] GetKey(ConsoleKey key, string[] map, Position pos, char[] charList, ref bool moving, int characterToMove){
        
        Console.WriteLine("Thread {0} (GetKey): {1}, Priority {2}",
                        Thread.CurrentThread.ManagedThreadId,
                        Thread.CurrentThread.ThreadState,
                        Thread.CurrentThread.Priority);

        //int command = 0;
        if(key == ConsoleKey.UpArrow){
            //Console.WriteLine("up");
            //command = 1;
            pos.PosY -= 1;
            pos.targetLocked = false;
        }
        if(key == ConsoleKey.RightArrow){
            //Console.WriteLine("Right");
            //command = 2;
            pos.PosX += 1;
            pos.targetLocked = false;
        }
        if(key == ConsoleKey.DownArrow){
            //Console.WriteLine("Down");
            //command = 3;
            pos.PosY += 1;
            pos.targetLocked = false;
        }
        if(key == ConsoleKey.LeftArrow){
            //Console.WriteLine("Left");
            //command = 4;
            pos.PosX -= 1;
            pos.targetLocked = false;
        }
        if(key == ConsoleKey.Enter){
            Console.WriteLine("Exiting...");
            //command = 100;
            moving = false;
        }
        if(key == ConsoleKey.Spacebar){
            int enemy = pos.CheckIfEnemy();
            Console.WriteLine("enemy " + enemy);
            if( enemy >= 0 ){
                if(pos.targetLocked){
                    pos.targetLocked = false;
                    Console.WriteLine("-Wait-");
                    if (OperatingSystem.IsWindows()){
                        for (int i = 0; i < 8; i++)
                        {
                            Console.Beep(2000,200);
                            Console.WriteLine("-Wait-");
                        }
                    }
                    EnemyFunctions.AttackEnemy(enemy);
                }else{
                    pos.targetLocked = true;
                    //EnemyFunctions.lockedEnemies[enemy] = true;
                    EnemyFunctions.enemyList.ElementAt(enemy).isTargeted = true;

                    if (OperatingSystem.IsWindows()){
                        Console.Beep(1000,200);
                        Console.Beep(1000,200);
                    }
                    Console.WriteLine("-Target locked!-");
                    //command = 5;
                    characterToMove = 5;
                }
            }else{
                pos.targetLocked = false;
                Console.WriteLine("-No Target-");

                if (OperatingSystem.IsWindows()){
                    Console.Beep(1200,150);
                    Console.Beep(1000,100);
                }
            }
        }
        string[] refreshMap = makeMap(charList, (20,10));
        string[] newMap = moveCharacter(refreshMap,pos,charList[characterToMove]);
        UpdateMap(newMap);
        //Console.WriteLine(Console.ReadKey(true).Key);
        return newMap;
    }

    public static void UpdateMap(string[] theMap){
        for (int i = 0; i < theMap.Length; i++)
        {           
            Console.WriteLine(theMap[i]);
        }
    }

    static void Main(string[] args)
    {
        //Tuple<int,int>[] enemies = EnemyFunctions.enemies;
        string[] theMap = makeMap(mapChars, mapSize);
        EnemyFunctions.EnemyPosUpdater(theMap);
        Position pos = new Position(0,0);
        UpdateMap(theMap);

        bool moving = true;
        //int keyCommand = 0;
        while( moving ){
            //UpdateMap(theMap, pos);
            theMap = GetKey(Console.ReadKey().Key, theMap, pos, mapChars, ref moving, 3);
            //Console.WriteLine(pos.PosX + "," + pos.PosY);
            //Console.WriteLine(pos.PosY);
            
        }
        
    }
}
