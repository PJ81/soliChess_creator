// See https://aka.ms/new-console-template for more information
using Microsoft.VisualBasic;
using System.Drawing;

Random rand = new();
const int MAXSIZE = 8;
string[,] board = new string[MAXSIZE, MAXSIZE];
string[,] temp = new string[MAXSIZE, MAXSIZE];
List<Tuple<Point, Point>> points = [];
int solved;
string[] PIECES = ["K", "Q", "B", "B", "N", "N", "R", "R", "P", "P", "B", "N", "R", "P", "B", "N", "R", "P", "B", "N", "R", "P"];

void shuffle(string[] arr) {
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

    int z = MAXSIZE == 4 ? 10 : MAXSIZE == 6 ? 14 : MAXSIZE == 7 ? 18 : 22;
    string[] pieces = new string[z];
    for (int a = 0; a < z; a++) {
        pieces[a] = PIECES[a];
    }

    for (int i = 0; i < z * 100; i++)
        shuffle(pieces);

    for (int i = 0; i < MAXSIZE; i++) {
        for (int j = 0; j < MAXSIZE; j++) {
            board[j, i] = "";
            temp[j, i] = "";
        }
    }

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

Point[] tryCapture(Point f) {
    string s = board[f.X, f.Y];

    Point[] moves = getMoves(s);
    if (moves.Length == 0) return [];

    Point t = new();

    if (s.Equals("K") || s.Equals("N") || s.Equals("P")) {
        for (int i = 0; i < moves.Length; i++) {
            t.X = f.X + moves[i].X;
            t.Y = f.Y + moves[i].Y;

            //if (inBounds(t) && board[t.X, t.Y] != "" && !t.Equals(f)) 
            if (!inBounds(t) || board[t.X, t.Y] == "" || t.Equals(f)) continue;
            board[t.X, t.Y] = s;
            board[f.X, f.Y] = "";
            return [f, t];
            
        }
    } else {
        for (int i = 0; i < moves.Length; i++) {
            t.X = f.X; 
            t.Y = f.Y;

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

void copyArray(string[,] from, string[,] to) {
    for (int i = 0; i < MAXSIZE; i++) {
        for (int j = 0; j < MAXSIZE; j++) {
            to[j, i] = from[j, i];
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

            // save board
            string[,] back = new string[MAXSIZE, MAXSIZE];
            copyArray(board, back);

            Point[] r = tryCapture(p);

            if (r.Length > 0) {
               
                Tuple<Point, Point> pts = new(r[0], r[1]);
                points.Add(pts);

                if (solveBoard()) return true;

                points.Remove(pts);
                copyArray(back, board);
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

