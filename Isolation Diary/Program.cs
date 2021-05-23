﻿using System;
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

            // Constructor
            public Page()
            {
                this.Text = new Text("Welcome to the Isolation Diary! \n");
                this.ControlGroups = new List<ControlGroup>();
                this.ControlGroups.Add(
                    new ControlGroup(
                        new Text("Health: "),
                        new List<ControlLine> {
                            new ControlLine(),
                            new ControlLine()
                        }
                        )
                    );
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
            }
        }

        public class ControlLine
        {
            // properties
            protected Text Text { get; set; }
            // TO DO! control

            // constructor
            public ControlLine()
            {
                this.Text = new Text("ControlLine");
            }

            // methods
            public void Print()
            {
                Console.WriteLine("ControlLine");
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