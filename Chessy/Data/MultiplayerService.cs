using Chessy.Models.Board;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chessy.Data;
public class MultiplayerService
{
    public Dictionary<string, Board> Games {  get; set; }
    private static Random random = new Random();

    public MultiplayerService()
    {
        Games = new Dictionary<string, Board>();
    }

    public Board GetBoard(string key)
    {
        if (!Games.ContainsKey(key))
            return null;

        return Games[key];
    }

    public void StartBoard(string key, Board board)
    {
        board.IsSecondPlayer = true;
        Games.Add(key, board);
    }

    public string GetKey()
    {
        string key = RandomString(3);
        while (Games.ContainsKey(key))
        {
            key = RandomString(3);
        }
        return key;
    }

    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
