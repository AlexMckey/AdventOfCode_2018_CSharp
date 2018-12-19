using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _09
{
    class MarbleGameCircle
    {
        private int playersCnt;
        private int curPlayer;
        private long[] players;
        private bool isScoreStep = false;
        private CircularDoubleLinkedList<int> dlCircular;

        public MarbleGameCircle(int pCnt)
        {
            playersCnt = pCnt;
            players = new long[playersCnt];
            dlCircular = new CircularDoubleLinkedList<int>(0);
            MarbleNum = 0;
            curPlayer = -1;
        }

        public int MarbleNum { get; private set; }

        public int CurrentPlayer => curPlayer + 1;

        public (int player, long score) WinnerScore => players
            .Select((v, i) => (idx: i + 1, score: v))
            .OrderByDescending(p => p.score)
            .First();

        public long MaxScore => WinnerScore.score;

        public void Go(int endMarble, bool printStep = false)
        {
            if (printStep)
                Console.WriteLine(this.ToString());
            while (MarbleNum <= endMarble)
            {
                this.NextStep();
                if (printStep)
                    Console.WriteLine(this.ToString());
            }
        }

        public void NextStep()
        {
            curPlayer = (curPlayer + 1) % playersCnt;
            if (++MarbleNum % 23 == 0)
                this.ScorePlayer();
            else
            {
                isScoreStep = false;
                dlCircular = dlCircular.MoveRight().Insert(MarbleNum);
            }
        }

        private void ScorePlayer()
        {
            isScoreStep = true;
            players[curPlayer] += MarbleNum;
            dlCircular = dlCircular.MoveLeft(7);
            players[curPlayer] += dlCircular.CurrentValue;
            dlCircular = dlCircular.Remove();
        }

        public override string ToString()
        {
            if (MarbleNum == 0)
                return "[--]:\t(0)";
            var player = CurrentPlayer < 10 ? $"[ {CurrentPlayer}]:\t" : $"[{CurrentPlayer}]:\t";
            var allMarble = dlCircular.Select(v => v).ToList();
            var head = string.Join("\t", allMarble.TakeWhile(v => v != MarbleNum));
            var cur = $"\t({MarbleNum})\t";
            var tail = string.Join("\t", allMarble.SkipWhile(v => v != MarbleNum).Skip(1));
            return player + head + (isScoreStep ? "" : cur) + tail;
        }
    }
}
