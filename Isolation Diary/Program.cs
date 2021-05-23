using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Isolation_Diary
{
    class Program
    {
        private static string FILENAME = "Isolation_Diary.csv";
        public static string[] attributes = {"Date", "Symptoms", "Mental", "SkillTime", "SkillRating"};
        public class Page
        {
            // Properties
            protected Text Text { get; set; }
            protected List<ControlGroup> ControlGroups { get; set; }
            protected ControlGroup FocusedControlGroup { get; set; }

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
                this.FocusedControlGroup = this.ControlGroups[0].FocusFirst();
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
                this.FocusedControlGroup.WaitForInput();
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
            protected ControlLine FocusedControlLine { get; set; }


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
            public ControlGroup FocusFirst()
            {
                this.Focused = true;
                this.FocusedControlLine = this.ControlLines[0].FocusFirst();
                return this;
            }

            internal void WaitForInput()
            {
                this.FocusedControlLine.WaitForInput();
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

            public ControlLine FocusFirst()
            {
                this.Focused = true;
                this.Control.Focus();
                return this;
            }
            internal void WaitForInput()
            {
                this.Control.WaitForInput();
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
            public abstract void WaitForInput();
        }

        public abstract class Field : Control
        {
           override public void Print()
            {
                if (this.Focused) {
                    Console.WriteLine("[[     ]]");
                }
                else
                {
                    Console.WriteLine("[     ]");
                }
            }
            public abstract override void WaitForInput();
        }

        public class FieldRating : Field
        {
            override public void WaitForInput()
            {
                string key = Console.ReadKey().KeyChar.ToString();

            }

        }

        public class FieldTime : Field
        {
            override public void WaitForInput()
            {
                string key = Console.ReadKey().KeyChar.ToString();

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
            override public void WaitForInput()
            {
                string key = Console.ReadKey().KeyChar.ToString();

                Console.WriteLine($"[[ {key} ]]");
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