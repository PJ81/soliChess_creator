// See https://aka.ms/new-console-template for more information
using Microsoft.VisualBasic;
using System.Drawing;

Random rand = new();
const int MAXSIZE = 4;
string[,] board = new string[MAXSIZE, MAXSIZE];
string[,] temp = new string[MAXSIZE, MAXSIZE];
List<Tuple<Point, Point>> points = [];


void createBoard() {

    int comparer(string a, string b) {
        return rand.Next(-1, 1);
    }

    string[] pieces = ["K", "Q", "B", "B", "N", "N", "R", "R", "P", "P"];
    
    for (int i = 0; i < 100; i++)
        Array.Sort(pieces, comparer);

    for (int i = 0; i < MAXSIZE; i++) {
        for (int j = 0; j < MAXSIZE; j++) {
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
        int x = rand.Next(MAXSIZE), y = rand.Next(MAXSIZE);
        board[x, y] = pieces[idx];
        temp[x, y] = pieces[idx++];   
    }
    */
}

string pt2String(Point p) {
    return Convert.ToString(Convert.ToChar(p.X + 65)) + Convert.ToString(Convert.ToChar(52 - p.Y));
}

void saveBoard() {
    StreamWriter w = new("boards.txt", true);

    for (int i = 0; i < MAXSIZE; i++) {
        for (int j = 0; j < MAXSIZE; j++) {
            string u = temp[j, i] == "" ? "-" : temp[j, i];
            w.Write(u + " ");
        }
        w.WriteLine();
    }

    w.Write(ControlChars.Tab);
    w.Write(ControlChars.Tab);

    foreach (Tuple<Point, Point> p in points) {
        w.Write(pt2String(p.Item1) + "->" + pt2String(p.Item2) + " ");
    }

    w.WriteLine();
    w.WriteLine();

    w.Close();
}

void printBoard() {
    for (int i = 0; i < MAXSIZE; i++) {
        for (int j = 0; j < MAXSIZE; j++) {
            string u = board[j, i] == "" ? "-" : board[j, i];
            Console.Write(u + " ");
        }
        Console.WriteLine();
    }

    Console.WriteLine();
    Console.WriteLine("--------- // ---------");
    Console.WriteLine();
}

bool inBounds(Point p) {
    return p.X >= 0 && p.Y >= 0 && p.X < MAXSIZE && p.Y < MAXSIZE;
}

int countPieces() {
    int count = 0;
    for (int i = 0; i < MAXSIZE; i++) {
        for (int j = 0; j < MAXSIZE; j++) {
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
            if (++x >= MAXSIZE) break;
        }
        x = 0;
        if (++y >= MAXSIZE) break;
    }
    return new Point(-1, -1);
}

Point[] getMoves(string s){
    return s switch {
        "K" or "Q" => [
                new(-1,  0),    new( 1,  0),
                new( 0, -1),    new( 0,  1),
                new(-1,  1),    new( 1, -1),
                new(-1, -1),    new( 1,  1)],
        "B" => [
                new(-1,  1),    new( 1, -1),
                new(-1, -1),    new( 1,  1)],
        "N" => [
                new(-2,  1),    new( 2,  1),
                new(-2, -1),    new( 2, -1),
                new(-1,  2),    new( 1,  2),
                new(-1, -2),    new( 1, -2)],
        "R" => [
               new(-1,  0),     new( 1,  0),
               new( 0, -1),     new( 0,  1)],
        "P" => [
               new( 1, -1),     new(-1, -1)],
        _ => [],
    };
}

Point[] tryCapture(Point f) {
    string s = board[f.X, f.Y];

    Point[] moves = getMoves(s);
    if (moves.Length == 0) return [];

    Point t = new();

    if (s.Equals("K") || s.Equals("N") || s.Equals("P")) {
        for (int i = 0; i < moves.Length; i++) {
            t.X = f.X + moves[i].X;
            t.Y = f.Y + moves[i].Y;

            if (inBounds(t) && board[t.X, t.Y] != "" && !t.Equals(f)) {
                board[t.X, t.Y] = s;
                board[f.X, f.Y] = "";
                return [f, t];
            }
        }
    } else {
        for (int i = 0; i < moves.Length; i++) {
            t.X = f.X; f.Y = f.Y;

            while (true) {
                t.X += moves[i].X; 
                t.Y += moves[i].Y;

                if (!inBounds(t)) break;

                if (board[t.X, t.Y] != "" && !t.Equals(f)) {
                    board[t.X, t.Y] = s;
                    board[f.X, f.Y] = "";
                    return [f, t];
                }
            }
        }
    }

    return [];
}

void copyArrays() {
    for (int i = 0; i < MAXSIZE; i++) {
        for (int j = 0; j < MAXSIZE; j++) {
            board[j, i] = temp[j, i];
        }
    }
}

bool solveBoard() {
    
    if (countPieces() == 1) {
        Console.WriteLine("-- Solved --!");
        return true;
    }

    for(int y = 0; y < MAXSIZE; y++) {
        for (int x = 0; x < MAXSIZE; x++) {
;
            Point p = new(x, y);
            Point[] r = tryCapture(p);

            if (r.Length > 0) {
                Tuple<Point, Point> pts = new(r[0], r[1]);
                points.Add(pts);

                if (solveBoard()) return true;

                points.Remove(pts);

                board[r[0].X, r[0].Y] = board[r[1].X, r[1].Y];
            }
        }
    }

    return false;
}

// entry point
for (int z = 0; z < 1; z++) {

    createBoard();
    printBoard();

    if (solveBoard()) {
        saveBoard();
    }
}

Console.WriteLine("Done!");

