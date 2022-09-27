// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using M;
using MusicGen;

using MusicComb;

// using Gradebook.book;

namespace GradeBook
{

    class Program
    {

        //quarter, whole, half, eighth, sixteenth

        //length == how many sixteenths are in the note

        public record struct NoteInformation(int length, string note, string keySig, MidiTimeSignature timeSig);
        const double sixteenth = 1;
        const double eighth = 2;
        const double quarter = 4;
        const double half = 8;
        const double whole = 16;
        static void Main(string[] args)
        {
            if(string.Compare(args[0], "generate") == 0){
                var filePath = args[1];
                var fileName = args[2];
                var clef = args[3];
                var instrument = args[4];
                var musicG = new MusicGenerator();
                musicG.generateMusic(filePath, fileName, clef, instrument);
                
                //Process cmd = new Process();
                
                Process.Start("lilypond",$"--png {fileName}.ly");
               // cmd.StartInfo.FileName = "lilypond.sh";
                //cmd.StartInfo.RedirectStandardInput = true;
                //cmd.StartInfo.RedirectStandardOutput = true;
                //cmd.StartInfo.CreateNoWindow = true;
                //cmd.StartInfo.UseShellExecute = false;
                //cmd.Start();

               // cmd.StandardInput.WriteLine("echo Oscar");
               // cmd.StandardInput.Flush();
               // cmd.StandardInput.Close();
               // cmd.WaitForExit();
             //   Console.WriteLine(cmd.StandardOutput.ReadToEnd());
            } else {
                var yours = args[1];
                var theirs = args[2];
                // var yourInstrument = args[3];
                var musicC = new MusicCombiner();
                musicC.musicCombiner(theirs, yours);

                Process.Start("lilypond","--png combined.ly");
                // Process cmd = new Process();
                // System.IO.File.WriteAllText(@"lilypond.sh",$"lilypond --png 'combined.ly'");
                // cmd.StartInfo.FileName = "lilypond.sh";
                // cmd.StartInfo.RedirectStandardInput = true;
                // cmd.StartInfo.RedirectStandardOutput = true;
                // cmd.StartInfo.CreateNoWindow = true;
                // cmd.StartInfo.UseShellExecute = false;
                // cmd.Start();

                // cmd.StandardInput.WriteLine("echo Oscar");
                // cmd.StandardInput.Flush();
                // cmd.StandardInput.Close();
                // cmd.WaitForExit();
                // Console.WriteLine(cmd.StandardOutput.ReadToEnd());
            }
        }
        
    }
}

