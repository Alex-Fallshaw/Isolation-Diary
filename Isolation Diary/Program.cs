using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Isolation_Diary
{

    class Program
    {
        public enum ProgramAction { NoAction, Accept, MoveNext, MovePrevious, Exit }
        private static string FILENAME = "Isolation_Diary.csv";
        public static string[] attributes = {"Date", "Symptoms", "Mental", "SkillTime", "SkillRating"};

        public class Page
        {
            // Properties
            protected Text Text { get; set; }
            protected List<ControlGroup> ControlGroups { get; set; }
            protected int FocusedControlGroup { get; set; }

            // Constructor
            public Page()
            {
                this.Text = new Text("Welcome to the Isolation Diary! \n");
                this.ControlGroups = new List<ControlGroup>();
                this.ControlGroups.Add(
                    new ControlGroup(
                        new Text("Health: "),
                        new List<ControlLine> {
                            new ControlLine(new Text("Rate your symptoms"), new FieldRating()),
                            new ControlLine(new Text("Rate your mental health"), new FieldRating())
                        }
                        )
                    );
                this.ControlGroups.Add(
                    new ControlGroup(
                        new Text("Skill: "),
                        new List<ControlLine> {
                            new ControlLine(new Text("How much practice today?"), new FieldTime()),
                            new ControlLine(new Text("Rate your proficiency"), new FieldRating())
                        }
                        )
                    ); this.ControlGroups.Add(
                        new ControlGroup(
                            new Text(""),
                            new List<ControlLine> {
                                new ControlLine(new Text(""), new Button())
                            }
                            )
                        );
                FocusFirst();
            }

            // Methods
            public void Print()
            {
                this.Text.Print();
                foreach (ControlGroup cg in this.ControlGroups)
                {
                    cg.Print();
                }
            }
            public void WaitForInput()
            {
                ProgramAction a = this.getFocusedControlGroup().WaitForInput();
                while (a != ProgramAction.Exit)
                {
                    if (a == ProgramAction.MoveNext) { this.MoveNextGroup(); }
                    else if (a == ProgramAction.MovePrevious) { this.MovePreviousGroup(); }
                    Console.Clear();
                    this.Print();
                    a = this.getFocusedControlGroup().WaitForInput();
                }
            }

            public bool canMoveNextGroup()
            {
                return this.FocusedControlGroup < (this.ControlGroups.Count - 1);
            }

            public bool canMovePreviousGroup()
            {
                return this.FocusedControlGroup > 0;
            }

            private void MoveNextGroup()
            {
                if (getFocusedControlGroup().canMoveNextLine())
                {
                    getFocusedControlGroup().MoveNextLine();
                }
                else if (canMoveNextGroup())
                {
                    getFocusedControlGroup().UnFocus();
                    this.FocusedControlGroup += 1;
                    getFocusedControlGroup().FocusFirst();
                }
                else
                {
                    FocusFirst();
                }
            }

            private void MovePreviousGroup()
            {
                if (getFocusedControlGroup().canMovePrevoiusLine())
                {
                    getFocusedControlGroup().MovePrevoiusLine();
                }
                else if (canMovePreviousGroup())
                {
                    getFocusedControlGroup().UnFocus();
                    this.FocusedControlGroup -= 1;
                    getFocusedControlGroup().FocusLast();
                }
                else
                {
                    FocusLast();
                }
            }

            private void FocusFirst()
            {
                getFocusedControlGroup().UnFocus();
                this.ControlGroups[0].FocusFirst();
                this.FocusedControlGroup = 0;
            }

            private void FocusLast()
            {
                getFocusedControlGroup().UnFocus();
                this.ControlGroups[-1].FocusLast();
                this.FocusedControlGroup = this.ControlGroups.Count - 1;
            }

            private ControlGroup getFocusedControlGroup()
            {
                return this.ControlGroups[this.FocusedControlGroup];
            }
        }

        public class Text
        {
            protected string Value { get; set; }

            public Text(string value)
            {
                this.Value = value;
            }

            public void Print()
            {
                Console.WriteLine(this.Value);
            }
        }

        public class ControlGroup
        {
            // Properties
            protected Text Text { get; set; }
            protected List<ControlLine> ControlLines { get; set; }
            protected Boolean Focused { get; set; }
            protected int FocusedControlLine { get; set; }

            // Constructor
            public ControlGroup(Text text, List<ControlLine> controlLines)
            {
                this.Text = text;
                this.ControlLines = controlLines;
            }

            // Methods
            public void Print()
            {
                this.Text.Print();
                foreach (ControlLine cl in this.ControlLines)
                {
                    cl.Print();
                }
                Console.Write("\n");
            }

            public void FocusFirst()
            {
                this.Focused = true;
                this.ControlLines[0].Focus();
                this.FocusedControlLine = 0;
            }

            public void FocusLast()
            {
                this.Focused = true;
                this.ControlLines[-1].Focus();
                this.FocusedControlLine = this.ControlLines.Count - 1;
            }

            internal ProgramAction WaitForInput()
            {
                return this.getFocusedControlLine().WaitForInput();
            }

            private ControlLine getFocusedControlLine()
            {
                return this.ControlLines[this.FocusedControlLine];
            }

            public bool canMoveNextLine()
            {
                return this.FocusedControlLine < (this.ControlLines.Count - 1);
            }

            public bool canMovePrevoiusLine()
            {
                return this.FocusedControlLine > 0;
            }

            public void MoveNextLine()
            {
                this.getFocusedControlLine().UnFocus();
                this.FocusedControlLine += 1;
                this.getFocusedControlLine().Focus();
            }

            public void MovePrevoiusLine()
            {
                this.getFocusedControlLine().UnFocus();
                this.FocusedControlLine -= 1;
                this.getFocusedControlLine().Focus();
            }

            public void UnFocus()
            {
                this.getFocusedControlLine().UnFocus();
                this.FocusedControlLine = -1;
                this.Focused = false;
            }
        }

        public class ControlLine
        {
            // properties
            protected Text Text { get; set; }
            protected Control Control { get; set; }
            protected Boolean Focused { get; set; }

            // constructor
            public ControlLine(Text text, Control control)
            {
                this.Text = text;
                this.Control = control;
            }

            // methods
            public void Print()
            {
                this.Text.Print();
                this.Control.Print();
            }

            public void Focus()
            {
                this.Focused = true;
                this.Control.Focus();
            }

            internal ProgramAction WaitForInput()
            {
                return this.Control.WaitForInput();
            }

            public void UnFocus()
            {
                this.Control.UnFocus();
                this.Focused = false;
            }
        }

        public abstract class Control
        {
            // properties
            protected Boolean Focused { get; set; }

            // methods
            public abstract void Print();

            public Control Focus()
            {
                this.Focused = true;
                return this;
            }

            public void UnFocus()
            {
                this.Focused = false;
            }

            public abstract ProgramAction WaitForInput();

            protected ProgramAction GetMovementFromConsoleKeyInfo(ConsoleKeyInfo key)
            {
                return ProgramAction.MoveNext;
            }

            protected bool IsMovementKey(ConsoleKeyInfo key)
            {
                // if (((key.Modifiers & ConsoleModifiers.Shift) != 0) & (key.Key == System.ConsoleKey.Tab))
                if ((key.Key == System.ConsoleKey.Tab))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            protected abstract bool IsAcceptKey(ConsoleKeyInfo key);

        }

        public abstract class Field : Control
        {
            override abstract public void Print();

            public abstract override ProgramAction WaitForInput();

            protected bool IsAppendKey(ConsoleKeyInfo key)
            {
                char[] appendChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

                // if (((key.Modifiers & ConsoleModifiers.Shift) != 0) & (key.Key == System.ConsoleKey.Tab))
                if (appendChars.Contains(key.KeyChar))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            protected bool IsRemoveKey(ConsoleKeyInfo key)
            {
                // if (((key.Modifiers & ConsoleModifiers.Shift) != 0) & (key.Key == System.ConsoleKey.Tab))
                if ((key.Key == System.ConsoleKey.Backspace))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            override protected bool IsAcceptKey(ConsoleKeyInfo key)
            {
                if (key.Key == System.ConsoleKey.Enter)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public class FieldRating : Field
        {
            override public ProgramAction WaitForInput()
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if (IsMovementKey(key))
                {
                    return GetMovementFromConsoleKeyInfo(key);
                }
                else if (IsAppendKey(key))
                {
                    return ProgramAction.NoAction;
                }
                else if (IsRemoveKey(key))
                {
                    return ProgramAction.NoAction;
                }
                else if (IsAcceptKey(key))
                {
                    return ProgramAction.MoveNext;
                }
                else
                {
                    return ProgramAction.NoAction;
                }
            }

            override public void Print()
            {
                if (this.Focused)
                {
                    Console.WriteLine("(0 to 9) [[     ]]");
                }
                else
                {
                    Console.WriteLine("(0 to 9)  [     ]");
                }
            }
        }

        public class FieldTime : Field
        {
            override public ProgramAction WaitForInput()
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if (IsMovementKey(key))
                {
                    return GetMovementFromConsoleKeyInfo(key);
                }
                else if (IsAppendKey(key))
                {
                    return ProgramAction.NoAction;
                }
                else if (IsRemoveKey(key))
                {
                    return ProgramAction.NoAction;
                }
                else if (IsAcceptKey(key))
                {
                    return ProgramAction.MoveNext;
                }
                else
                {
                    return ProgramAction.NoAction;
                }
            }

            override public void Print()
            {
                if (this.Focused)
                {
                    Console.WriteLine("(minutes) [[     ]]");
                }
                else
                {
                    Console.WriteLine("(minutes)  [     ]");
                }
            }
        }

        public class Button : Control
        {
            override public void Print()
            {
                if (this.Focused)
                {
                    Console.WriteLine("[[ EXIT ]]");
                }
                else
                {
                    Console.WriteLine("[ EXIT ]");
                }
            }
            override public ProgramAction WaitForInput()
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if (IsMovementKey(key))
                {
                    return GetMovementFromConsoleKeyInfo(key);
                }
                else if (IsAcceptKey(key))
                {
                    return ProgramAction.Accept;
                }
                else
                {
                    return ProgramAction.NoAction;
                }
            }

            override protected bool IsAcceptKey(ConsoleKeyInfo key)
            {
                if ((key.Key == System.ConsoleKey.Enter) | (key.KeyChar == ' '))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        static void WriteDB(Hashtable db)
        {
            // Write data to CSV file
            using (CsvFileWriter writer = new CsvFileWriter(FILENAME))
            {
                foreach (string k in db.Keys)
                {
                    Hashtable record = db[k] as Hashtable;
                    CsvRow row = new CsvRow();
                    for (int i = 0; i < attributes.Length; i++)
                    {
                        row.Add(record[attributes[i]] as string);
                    }
                    writer.WriteRow(row);
                }
            }
        }

        
        static Hashtable ReadDB()
        {
            //Returns an empty table if the file doesn't exist.
            if (!File.Exists(FILENAME)) return new Hashtable();

            Hashtable db = new Hashtable();

            // Read data from CSV file
            using (CsvFileReader reader = new CsvFileReader(FILENAME))
            {
                CsvRow row = new CsvRow();
                while (reader.ReadRow(row))
                {
                    Hashtable record = new Hashtable();
                    for (int i = 0; i < attributes.Length; i++)
                    {
                        record[attributes[i]] = row[i];
                    }
                    db[record[attributes[0]]] = record;
                }
            }

            return db;
        }

        static void Main(string[] args)
        {
            Page page = new Page();
            page.Print();
            page.WaitForInput();

        }
    }
}