// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using M;
using MusicGen;

namespace MusicComb 
{
    class MusicCombiner {
        //your music 
        public void musicCombiner(string filePath1, string filePath2){
            var theirLines = File.ReadAllLines(filePath1).ToList();
            var yourLines = File.ReadAllLines(filePath2).ToList();
            using(StreamWriter writer = new StreamWriter("combined.ly")){
                // writer.WriteLine("\\new Staff \\with {\n");
                // writer.WriteLine($"instrumentName = \"{yourInstrument}\"\n");
                // writer.WriteLine("}\n");
                foreach(string s in yourLines){
                    writer.WriteLine(s);
                } 
                foreach(string s1 in theirLines){
                    writer.WriteLine(s1);

                }
            }
            
            

            // System.IO.File.WriteAllText(@"combined.ly", "\\new Staff \\with {\n");
            // System.IO.File.WriteAllText(@"combined.ly",$"instrumentName = \"{yourInstrument}\"\n");
            // System.IO.File.WriteAllText(@"combined.ly", "}\n");
            // foreach(string s in yourLines){
            //     System.IO.File.WriteAllText(@"combined.ly", s);
            // } 
            // foreach(string s1 in theirLines){
            //     System.IO.File.WriteAllText(@"combined.ly", s1);

            // }
            // System.IO.File.WriteAllLines(@"combined.ly", yourLines);
            // System.IO.File.WriteAllLines(@"combined.ly", theirLines);
        }




        


    }
}