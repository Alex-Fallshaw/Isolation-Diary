﻿using System;
using System.Collections;
using System.IO;

namespace Isolation_Diary
{
    class Program
    {
        private static string FILENAME = "Isolation_Diary.csv";
        public static string[] attributes = {"Date", "Symptoms", "Mental", "SkillTime", "SkillRating"};
        public class Page
        {
            public void Print()
            {
                Console.WriteLine("page");
            }
            public void WaitForInput()
            {

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

        //PLEASE ADD YOUR OWN COMMENTS EXPLAINING WHAT THIS DOES
        //Note: this is like "loading game"!
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