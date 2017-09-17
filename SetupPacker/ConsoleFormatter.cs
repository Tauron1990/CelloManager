using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SetupPacker
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class ConsoleFormatter
    {
        private class ConsoleLine
        {
            public ConsoleColor Color { get; set; }
            public string Line { get; set; }
        }

        private class ListConsoleLine : ConsoleLine
        {
            public string Identifer { get; set; }

            public string Title { get; set; }

            public string EndLabel { get; set; }

            public int Lenght { get; set; }
            
            public void PrepareLine()
            {
                int? padLenght = Lenght - Title.Length - EndLabel?.Length;

                if (padLenght == null)
                {
                    Line = Title;
                    return;
                }

                int topad = padLenght.Value;
                if (topad <= 0)
                {
                    Line = Title + " " + EndLabel;
                    return;
                }

                Line = Title + EndLabel?.PadLeft(padLenght.Value + EndLabel.Length);
            }
        }

        public string ProgrammTitle
        {
            get => Console.Title;
            set => Console.Title = value;
        }

        public int ConsoleWith
        {
            get => Console.BufferWidth;
            set => Console.BufferWidth = value;
        }

        public string Title { get; set; }

        public ConsoleColor DefaultColor { get; set; }

        private readonly List<ConsoleLine> _lines;

        public ConsoleFormatter()
        {
            DefaultColor = ConsoleColor.White;
            _lines = new List<ConsoleLine>();
        }

        public void Initialize()
        {
            ConsoleWith = Console.WindowWidth;
            Reset();
        }

        public void WriteLine()
        {
            WriteCore(String.Empty, DefaultColor, false, true);
        }

        public void WriteLine(string line) => WriteCore(line, DefaultColor, false, true);

        public void WriteLine(string line, ConsoleColor color) => WriteCore(line, color, false, true);

        public void WriteLine(string line, ConsoleColor color, bool pad) => WriteCore(line, color, pad, true);

        public void Write(string line) => WriteCore(line, DefaultColor, false, false);

        public void Write(string line, ConsoleColor color) => WriteCore(line, color, false, false);

        public void Write(string line, ConsoleColor color, bool pad) => WriteCore(line, color, pad, false);

        public void Clear()
        {
            _lines.Clear();
            Reset();
        }

        public void AddListLabel(string identifer, string titel, int lenght = -1, ConsoleColor color = ConsoleColor.White)
        {
            if (lenght <= 0)
                lenght = ConsoleWith;

            if(FindLine(identifer) != null) return;

            ListConsoleLine line = new ListConsoleLine { Identifer = identifer, Color = color, Title = titel, Lenght = lenght};
            line.PrepareLine();

            PrintConsoleLine(line);
            _lines.Add(line);
        }

        public void SetListLabel(string identifer, string endLabel, bool last = true)
        {
            var line = FindLine(identifer);
            if(line == null) return;

            line.EndLabel = endLabel;
            line.PrepareLine();

            if(last) WriteLine();

            Reset();
        }

        private ListConsoleLine FindLine(string identifer) => _lines.OfType<ListConsoleLine>().FirstOrDefault(l => l.Identifer == identifer);

        private void WriteCore(string line, ConsoleColor color, bool pad, bool newLine)
        {
            if (pad)
                line = PadString(line);
            if (newLine)
                line = line + Environment.NewLine;

            ConsoleLine newline = new ConsoleLine { Color = color, Line = line };
            PrintConsoleLine(newline);
            _lines.Add(newline);
        }

        private void Reset()
        {
            Console.Clear();
            if (!string.IsNullOrWhiteSpace(Title))
                Console.WriteLine(PadString(Title, '_'));

            foreach (var line in _lines)
                PrintConsoleLine(line);
        }

        private void PrintConsoleLine(ConsoleLine line)
        {
            bool set = line.Color != DefaultColor;
            if (set)
                Console.ForegroundColor = line.Color;

            Console.Write(line.Line);

            if (set)
                Console.ForegroundColor = DefaultColor;

        }

        private string PadString(string line, char paddingChar = ' ')
        {
            int bufferLenght = ConsoleWith;
            int topad = bufferLenght - line.Length;
            if (topad <= 0) return line;
            
            int left = topad / 2;

            return line.PadLeft(left + line.Length / 2, paddingChar).PadRight(topad, paddingChar);
        }
    }
}
