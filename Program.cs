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

class Position{
    public Position(int x, int y){
        PosX = x;
        PosY = y;
    }
    public int PosX {get;set;}
    public int PosY {get;set;}
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
                        chars[oneEnemy.Item1] = charList[2]; //pos x
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
            //Console.WriteLine("Attack");
            command = 5;
            characterToMove = 4;
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

        Tuple<int,int>[] enemies = new Tuple<int, int>[] { 
            new Tuple<int,int>(15,3),
            new Tuple<int,int>(4,4),
            new Tuple<int,int>(1,1),
        };
        string[] theMap = makeMap(mapChars, (20,10), enemies);
        Position pos = new Position(1,1);
        UpdateMap(theMap);

        bool moving = true;
        int keyCommand = 0;
        while( moving ){
            //UpdateMap(theMap, pos);
            theMap = GetKey(Console.ReadKey().Key, theMap, pos, mapChars, ref moving, 3, enemies);
            //Console.WriteLine(pos.PosX);
            //Console.WriteLine(pos.PosY);
            
        }
        
    }
}
