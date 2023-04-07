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

class EnemyFunctions{
    static public Tuple<int,int>[] enemies = new Tuple<int, int>[] { 
        new Tuple<int,int>(15,3),
        new Tuple<int,int>(4,4),
        new Tuple<int,int>(2,2),
    };
    static public bool AttackEnemy(int enemyIndex){
        if (OperatingSystem.IsWindows()){
            Console.Beep(500,800);
        }
        enemies[enemyIndex] = new Tuple<int,int>(-1,-1);

        Console.WriteLine("-Destroyed-");
        return true;
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

    public int CheckIfEnemy(Tuple<int,int>[] enemies){
        int targetOk = -1;
        int index = -1;
        foreach (var oneEnemy in enemies)
        {
            index++;
            if(oneEnemy.Item1 == PosX && oneEnemy.Item2 == PosY)
                targetOk = index;
        }
        return targetOk;
    }
}

class Program
{
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

    static string[] makeMap(char[] charList, (int,int) size, Tuple<int,int>[] enemies){
        string[] map = new string[size.Item2];
        for (int i = 0; i < size.Item2; i++)
        {
            if(i == 0 || i == size.Item2-1){
                string border = new String(charList[0], size.Item1);
                map[i] = charList[0] + border + charList[0];
            }else{
                string space = new String(charList[1], size.Item1);

                foreach (var oneEnemy in enemies)
                {
                    if(i == oneEnemy.Item2){ //pos y
                        char[] chars = space.ToCharArray();
                        chars[oneEnemy.Item1-1] = charList[2]; //pos x
                        space = new string(chars);
                    }               
                }

                map[i] = charList[0] + space + charList[0];
            }
        }
        return map;
    }

    static string[] GetKey(ConsoleKey key, string[] map, Position pos, char[] charList, ref bool moving, int characterToMove, Tuple<int,int>[] enemies){
        int command = 0;
        if(key == ConsoleKey.UpArrow){
            //Console.WriteLine("up");
            command = 1;
            pos.PosY -= 1;
        }
        if(key == ConsoleKey.RightArrow){
            //Console.WriteLine("Right");
            command = 2;
            pos.PosX += 1;
        }
        if(key == ConsoleKey.DownArrow){
            //Console.WriteLine("Down");
            command = 3;
            pos.PosY += 1;
        }
        if(key == ConsoleKey.LeftArrow){
            //Console.WriteLine("Left");
            command = 4;
            pos.PosX -= 1;
        }
        if(key == ConsoleKey.Enter){
            //Console.WriteLine("Stop");
            command = 100;
            moving = false;
        }
        if(key == ConsoleKey.Spacebar){
            int enemy = pos.CheckIfEnemy(enemies);
            //Console.WriteLine("enemy " + enemy);
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
                    if (OperatingSystem.IsWindows()){
                        Console.Beep(1000,200);
                        Console.Beep(1000,200);
                    }
                    Console.WriteLine("-Target locked!-");
                    command = 5;
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
        string[] refreshMap = makeMap(charList, (20,10), enemies);
        string[] newMap = moveCharacter(refreshMap,pos,charList[characterToMove]);
        UpdateMap(newMap);
        //Console.WriteLine(Console.ReadKey(true).Key);
        return newMap;
    }

    static void UpdateMap(string[] theMap){
        for (int i = 0; i < theMap.Length; i++)
        {           
            Console.WriteLine(theMap[i]);
        }
    }

    static void Main(string[] args)
    {
        char[] mapChars = new char[] {
            '#',    //0 border
            ' ',    //1 space
            '¤',    //2 enemy
            '+',    //3 cursor
            '*',    //4 attack
            'X'     //5 lock
        };

        string[] theMap = makeMap(mapChars, (20,10), EnemyFunctions.enemies);
        Position pos = new Position(0,0);
        UpdateMap(theMap);

        bool moving = true;
        int keyCommand = 0;
        while( moving ){
            //UpdateMap(theMap, pos);
            theMap = GetKey(Console.ReadKey().Key, theMap, pos, mapChars, ref moving, 3, EnemyFunctions.enemies);
            //Console.WriteLine(pos.PosX + "," + pos.PosY);
            //Console.WriteLine(pos.PosY);
            
        }
        
    }
}
