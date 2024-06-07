// See https://aka.ms/new-console-template for more information

#region properties
const int MAXSIZE = 4;
string[] PIECES = ["K", "Q", "B", "B", "N", "N", "R", "R", "P", "P", "B", "N", "R", "P", "B", "N", "R", "P", "B", "N", "R", "P", "B", "N", "R", "P"];
string[,] board = new string[MAXSIZE, MAXSIZE];
string[,] temp = new string[MAXSIZE, MAXSIZE];
List<Tuple<Point, Point>> points = [];
int solvedCount;
Random rand = new();

#endregion

void shuffleArray(string[] arr, int iterations = 1) {

    for (int i = 0; i < iterations; i++) {

        int i1 = rand.Next(arr.Length);
        int i2 = rand.Next(arr.Length);

        (arr[i1], arr[i2]) = (arr[i2], arr[i1]);
    }
}

void createBoard() {

    int pieceCount = MAXSIZE == 4 ? 10 : MAXSIZE == 5 ? 14 : MAXSIZE == 6 ? 18 : MAXSIZE == 7 ? 22 : 26;

    string[] pieces = new string[pieceCount];
    Array.Copy(PIECES, pieces, pieceCount);

    shuffleArray(pieces, pieceCount * 100);

    for (int i = 0; i < MAXSIZE; i++) {
        for (int j = 0; j < MAXSIZE; j++) {
            board[j, i] = "";
            temp[j, i] = "";
        }
    }

    int count = rand.Next(MAXSIZE, pieces.Length + 1);

    for (int i = 0; i < count; i++) {
        while (true) {

            int x = rand.Next(MAXSIZE), y = rand.Next(MAXSIZE);

            if (board[x, y] == "") {
                board[x, y] = pieces[i];
                temp[x, y] = pieces[i];
                break;
            }
        }
    }
}

string pt2String(Point p) {
    return $"{(char)(p.X + 65)}{(char)(48 + MAXSIZE - p.Y)}";
}

void saveBoard() {
    using (StreamWriter writer = new StreamWriter($"boards{MAXSIZE}.txt", true)) {
        for (int i = 0; i < MAXSIZE; i++) {
            for (int j = 0; j < MAXSIZE; j++) {
                string u = string.IsNullOrEmpty(temp[j, i]) ? "-" : temp[j, i];
                writer.Write(u + " ");
            }
            writer.WriteLine();
        }

        writer.Write("\t\t");

        foreach (var point in points) {
            writer.Write($"{pt2String(point.Item1)}->{pt2String(point.Item2)} ");
        }

        writer.WriteLine();
        writer.WriteLine();

        writer.Close();
    }
}

void printBoard() {
    for (int i = 0; i < MAXSIZE; i++) {
        for (int j = 0; j < MAXSIZE; j++) {
            string u = string.IsNullOrEmpty(temp[j, i]) ? "-" : temp[j, i];
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
    return board.Cast<string>().Count(cell => !string.IsNullOrEmpty(cell));
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

    Point origin = new(x, y);
    
    if (s.Equals("K") || s.Equals("N") || s.Equals("P")) {

        foreach (Point move in moves) {
            Point target = new(origin.X + move.X, origin.Y + move.Y);

            if (inBounds(target) && board[target.X, target.Y] != "" && !target.equals(origin))
                possibilities.Add(target);
        }
    } else {
        foreach (Point move in moves) {
            int a = x, b = y;
            while (true) {
                a += move.X;
                b += move.Y;

                Point target = new(a, b);
                if (!inBounds(target)) break;

                if (board[target.X, target.Y] != "" && !target.equals(origin)) {
                    possibilities.Add(target);
                    break;
                }
            }
        }
    }

    return possibilities;
}

bool solveBoard() {
    
    if (countPieces() == 1) {
        Console.WriteLine("[Solved!]");
        return true;
    }

    for(int y = 0; y < MAXSIZE; y++) {
        for (int x = 0; x < MAXSIZE; x++) {

            if (board[x, y] == "") continue;

            List<Point> moves = tryCapture(x, y);

            if(moves.Count == 0) continue;

            foreach(Point pt in moves) {

                Tuple<Point, Point> move = new(new Point(x, y), pt);
                points.Add(move);

                string f = board[move.Item1.X, move.Item1.Y],
                        t = board[move.Item2.X, move.Item2.Y];

                board[move.Item2.X, move.Item2.Y] = f;
                board[move.Item1.X, move.Item1.Y] = "";

                if (solveBoard()) return true;

                board[move.Item2.X, move.Item2.Y] = t;
                board[move.Item1.X, move.Item1.Y] = f;

                points.Remove(move);

            }
        }
    }

    return false;
}

// entry point
solvedCount = 0;

for (int z = 0; z < 10; z++) {

    createBoard();
    printBoard();

    points.Clear();

    if (solveBoard()) {
        saveBoard();
        solvedCount++;
    } else {
        Console.WriteLine("[Not solved!]");
    }

    Console.WriteLine("--------- // ---------\n");
}

Console.WriteLine($"Created {solvedCount} boards!");

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