// See https://aka.ms/new-console-template for more information
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

Random rand = new Random();
string[,] board = new string[4, 4];
string[,] temp = new string[4, 4];

void createBoard() {

    int comparer(string a, string b) {
        return rand.Next(-1, 1);
    }

    string[] pieces = ["K", "Q", "B", "B", "N", "N", "R", "R", "P", "P"];
    
    for (int i = 0; i < 100; i++)
        Array.Sort(pieces, comparer);

    for (int i = 0; i < 4; i++) {
        for (int j = 0; j < 4; j++) {
            board[j, i] = "";
            temp[j, i] = "";
        }
    }

    temp[0, 0] = board[0, 0] = "N";
    temp[2, 0] = board[2, 0] = "P";
    temp[3, 0] = board[3, 0] = "B";
    temp[0, 2] = board[0, 2] = "N";
    temp[2, 2] = board[2, 2] = "Q";
    temp[3, 2] = board[3, 2] = "K";
    temp[0, 3] = board[0, 3] = "R";
    temp[1, 3] = board[1, 3] = "B";
    temp[2, 3] = board[2, 3] = "R";
    temp[3, 3] = board[3, 3] = "P";

    /*

    int count = rand.Next(7) + 4, idx = 0;

    for (int i = 0; i < count; i++) {
        int x = rand.Next(4), y = rand.Next(4);
        board[x, y] = pieces[idx];
        temp[x, y] = pieces[idx++];   
    }
    */
}

bool inBounds(Point p) {
    return p.X >= 0 && p.Y >= 0 && p.X < 4 && p.Y < 4;
}

void printBoard() {
    for (int i = 0; i < 4; i++) {
        for (int j = 0; j < 4; j++) {
            string u = board[j, i] == "" ? "-" : board[j, i];
            Console.Write(u + " ");
        }
        Console.WriteLine();
    }

    Console.WriteLine();
    Console.WriteLine("--------- // ---------");
    Console.WriteLine();
}

int countPieces() {
    int count = 0;
    for (int i = 0; i < 4; i++) {
        for (int j = 0; j < 4; j++) {
            count += board[j, i] != "" ? 1 : 0;
        }
    }
    return count;
}

Point getNextPiece(Point p) {
    int x = p.X, y = p.Y;

    while(true) {
        while(true) {
            if (board[x, y] != "") {
                return new Point(x, y);
            }
            if (++x >= 4) break;
        }
        x = 0;
        if (++y >= 4) break;
    }
    return new Point(-1, -1);
}

Point[] getMoves(string p){
    switch (p) {
        case "K":
        case "Q":
            return [
                new(-1,  0),    new( 1,  0),
                new( 0, -1),    new( 0,  1),
                new(-1,  1),    new( 1, -1),
                new(-1, -1),    new( 1,  1)];
        case "B":                     
           return [                  
                new(-1,  1),    new( 1, -1),
                new(-1, -1),    new( 1,  1)];
        case "N":                     
           return [                  
                new(-2,  1),    new( 2,  1),
                new(-2, -1),    new( 2, -1),
                new(-1,  2),    new( 1,  2),
                new(-1, -2),    new( 1, -2)];
        case "R":                     
           return [                  
               new(-1,  0),     new( 1,  0),
               new( 0, -1),     new( 0,  1)];
        case "P":
            return [
               new( 1, -1),     new(-1, -1)];
    }
    return [];
}

bool tryCapture(Point p) {
    string s = board[p.X, p.Y];
    Point[] moves = getMoves(s);
    if (moves.Length == 0) return false;
    Point t = new();

    if (s.Equals("K") || s.Equals("N") || s.Equals("P")) {
        for (int i = 0; i < moves.Length; i++) {
            t.X = p.X + moves[i].X;
            t.Y = p.Y + moves[i].Y;

            if (!inBounds(t)) break;

            if (board[t.X, t.Y] != "") {
                board[t.X, t.Y] = s;
                board[p.X, p.Y] = "";
                return true;
            }
        }
    } else {
        for (int i = 0; i < moves.Length; i++) {
            t.X = p.X; p.Y = p.Y;

            while (true) {
                t.X += moves[i].X; 
                t.Y += moves[i].Y;

                if (!inBounds(t)) break;

                if (board[t.X, t.Y] != "") {
                    board[t.X, t.Y] = s;
                    board[p.X, p.Y] = "";
                    return true;
                }
            }
        }
    }

    return false;
}

void copyArrays() {
    for (int i = 0; i < 4; i++) {
        for (int j = 0; j < 4; j++) {
            board[j, i] = temp[j, i];
        }
    }
}

bool solveBoard() {
    
    if (countPieces() == 1) {
        Console.WriteLine("-- Solved --!");
        return true;
    }

    for(int y = 0; y < 4; y++) {
        for (int x = 0; x < 4; x++) {
;
            Point p = new(x, y);

            p = getNextPiece(p);

            if (p.X < 0) return false;

            if (tryCapture(p)) {
                if(!solveBoard()) break;
                return true;
            }
        }
    }

    return false;
}

createBoard();

for (int z = 0; z < 1; z++) {

    printBoard();

    if (solveBoard()) {

        StreamWriter w = new StreamWriter("boards.txt", true);

        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                string u = temp[j, i] == "" ? "-" : temp[j, i];
                w.Write(u + " ");
            }
            w.WriteLine();
        }

        w.WriteLine();

        w.Close();
    }
}

Console.WriteLine("Done!");

