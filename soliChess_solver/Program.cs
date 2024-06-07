// See https://aka.ms/new-console-template for more information
using Microsoft.VisualBasic;
using System.Drawing;

Random rand = new();
const int MAXSIZE = 8;
string[,] board = new string[MAXSIZE, MAXSIZE];
string[,] temp = new string[MAXSIZE, MAXSIZE];
List<Tuple<Point, Point>> points = [];

int solved;
string[] PIECES = ["K", "Q", "B", "B", "N", "N", "R", "R", "P", "P", "B", "N", "R", "P", "B", "N", "R", "P", "B", "N", "R", "P", "B", "N", "R", "P"];

void shuffleArray(string[] arr) {
    int n = arr.Length;
    string tmp;

    for (int i = n - 1; i > 0; i--) {

        int j = rand.Next(i + 1);

        tmp = arr[i];
        arr[i] = arr[j];
        arr[j] = tmp;
    }
}

void createBoard() {

    int z = MAXSIZE == 4 ? 10 : MAXSIZE == 5 ? 14 : MAXSIZE == 6 ? 18 : MAXSIZE == 7 ? 22 : 26;
    string[] pieces = new string[z];
    for (int a = 0; a < z; a++) {
        pieces[a] = PIECES[a];
    }

    for (int i = 0; i < z * 100; i++)
        shuffleArray(pieces);

    for (int i = 0; i < MAXSIZE; i++) {
        for (int j = 0; j < MAXSIZE; j++) {
            board[j, i] = "";
            temp[j, i] = "";
        }
    }

    /*
    board[1,0] = temp[1,0] = "N";
    board[2,0] = temp[2,0] = "B";

    board[2,1] = temp[2,1] = "B";
    board[3,1] = temp[3,1] = "P";

    board[0,2] = temp[0,2] = "R";
    board[1,2] = temp[1,2] = "R";
    board[3,2] = temp[3,2] = "P";

    board[0, 3] = temp[0, 3] = "N";

    return;*/

    int count = rand.Next(MAXSIZE, pieces.Length), idx = 0;

    for (int i = 0; i < count; i++) {
        while (true) {

            int x = rand.Next(MAXSIZE), y = rand.Next(MAXSIZE);

            if (board[x, y] == "") {
                board[x, y] = pieces[idx];
                temp[x, y] = pieces[idx++];
                break;
            }
        }
    }
}

string pt2String(Point p) {
    return Convert.ToString(Convert.ToChar(p.X + 65)) + Convert.ToString(Convert.ToChar(48 + MAXSIZE - p.Y));
}

void saveBoard() {
    StreamWriter w = new("boards" + MAXSIZE + ".txt", true);

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

List<Point> tryCapture(int x, int y) {
    string s = board[x, y];

    List<Point> possibilities = [];

    Point[] moves = getMoves(s);
    if (moves.Length == 0) return [];

    Point f = new(x, y);
    

    if (s.Equals("K") || s.Equals("N") || s.Equals("P")) {
        for (int i = 0; i < moves.Length; i++) {
            Point t = new(f.X + moves[i].X, f.Y + moves[i].Y);

            if (!inBounds(t) || board[t.X, t.Y] == "" || t.equals(f)) continue;

            possibilities.Add(t);
        }
    } else {
        for (int i = 0; i < moves.Length; i++) {
            int a = x, b = y;
            while (true) {
                a += moves[i].X;
                b += moves[i].Y;

                Point t = new(a, b);
                if (!inBounds(t)) break;

                if (board[t.X, t.Y] != "" && !t.equals(f)) {
                    possibilities.Add(t);
                    break;
                }
            }
        }
    }

    return possibilities;
}

bool solveBoard() {
    
    if (countPieces() == 1) {
        Console.WriteLine("-- Solved --!");
        return true;
    }

    for(int y = 0; y < MAXSIZE; y++) {
        for (int x = 0; x < MAXSIZE; x++) {

            if (board[x, y] == "") continue;

            List<Point> moves = tryCapture(x, y);

            if(moves.Count == 0) continue;

            foreach(Point pt in moves) {

                Tuple<Point, Point> pts = new(new Point(x, y), pt);
                points.Add(pts);

                string f = board[pts.Item1.X, pts.Item1.Y],
                        t = board[pts.Item2.X, pts.Item2.Y];

                board[pts.Item2.X, pts.Item2.Y] = f;
                board[pts.Item1.X, pts.Item1.Y] = "";

                if (solveBoard()) return true;

                board[pts.Item2.X, pts.Item2.Y] = t;
                board[pts.Item1.X, pts.Item1.Y] = f;

                points.Remove(pts);

            }
        }
    }

    return false;
}

// entry point
solved = 0;

for (int z = 0; z < 10; z++) {

    createBoard();
    printBoard();

    points.Clear();

    if (solveBoard()) {
        solved++;
        saveBoard();
    }

    Console.WriteLine("--------- // ---------");
    Console.WriteLine();
}

Console.WriteLine("Done " + solved + " boards!");

class Point {
    public int X, Y;

    public bool equals(Point o) {
        return X == o.X && Y == o.Y;
    }

    public Point(int x = 0, int y = 0) {
        X = x;
        Y = y;
    }
}