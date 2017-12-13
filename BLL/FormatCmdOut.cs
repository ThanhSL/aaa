using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    
    public class FormatCmdOut
    {
        public void ClearToEndOfCurrentLine()
        {
            int currentLeft = Console.CursorLeft;
            int currentTop = Console.CursorTop;
            Console.Write(new String(' ', Console.WindowWidth - currentLeft));
            Console.SetCursorPosition(currentLeft, currentTop);
        }

        public void ClearFromPosToEndOfCurrentLine(int pos)
        {
            int currentTop = Console.CursorTop;
            Console.SetCursorPosition(pos, currentTop);
            Console.Write(new String(' ', Console.WindowWidth - pos));
            Console.SetCursorPosition(pos, currentTop);
        }


        public void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public void customWriteLine(string strOut)
        {
            int currentLineCursor = Console.CursorTop - 1;
            Console.SetCursorPosition(0, currentLineCursor);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
            Console.WriteLine(strOut);
        }


    }
}
