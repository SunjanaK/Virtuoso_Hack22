// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using M;

// using Gradebook.book;

namespace MusicGen
{

    class MusicGenerator
    {

        //quarter, whole, half, eighth, sixteenth

        //length == how many sixteenths are in the note

        public record struct NoteInformation(int length, string note, string keySig, MidiTimeSignature timeSig);
        const double sixteenth = 1;
        const double eighth = 2;
        const double quarter = 4;
        const double half = 8;
        const double whole = 16;

        public void generateMusic(string filePath, string fileName, string clef, string instrument)
        {
            int[] rhythms = new[]{1, 2, 4, 8, 16};
            
            // Console.WriteLine("Enter path to midi file");
            string input = filePath;
            var newMidiFile = MidiFile.ReadFrom(input);
            var newMidiSeqs = newMidiFile.Tracks;
            var timeBase = newMidiFile.TimeBase/4;
            // Console.WriteLine(newMidiSeqs.Count);
            var sb = new StringBuilder();
            sb.AppendLine("\\new Staff \\with {\n");
            sb.AppendLine($"instrumentName = \"{instrument}\"\n");
            sb.AppendLine("}\n");
            sb.AppendLine("{");
            sb.AppendLine($"\\time {newMidiSeqs[0].TimeSignature}");
            string keyOf = newMidiSeqs[0].KeySignature.ToString().Split(' ')[0].ToLower();
            string keyAlteration = newMidiSeqs[0].KeySignature.ToString().Split(' ')[1];
            sb.Append($"\\key {keyOf}");
            sb.Append($" \\{keyAlteration}\n");
            var noteInfs = new List<NoteInformation>();
            //format the major, need to split by space

            foreach(var seq in newMidiSeqs){
                Console.WriteLine(seq.KeySignature);
                Console.WriteLine(seq.TimeSignature);
                var midinotes = seq.ToNoteMap();
                 foreach(var note in midinotes){
                     //removing noise
                    if(int.Parse(note.Velocity.ToString()) < 50){
                        continue;
                    }
                    // Console.WriteLine($"Note is {note.Note}");
                    var numSixteenths = note.Length/timeBase;
                    Console.WriteLine(note.Note);
                    Console.WriteLine(note.Length);
                    
                    Console.WriteLine(note.Velocity);

                    
                    var compare = new []{ 
                        Math.Abs(sixteenth-numSixteenths), 
                        Math.Abs(eighth-numSixteenths), 
                        Math.Abs(quarter-numSixteenths), 
                        Math.Abs(half-numSixteenths),
                        Math.Abs(whole-numSixteenths) 
                        };
                    var low = compare.Min();
                    int index = 0;
                    foreach(var c in compare){
                        if(c != low){
                            index += 1;
                        } else {
                            break;
                        }
                    }
                    //the estimated number of sixteenths that maps directly to rhythm val 
                    var rhythmVal = rhythms[index];
                    var newInfo = new NoteInformation(){
                        length = rhythms[index],
                        note = note.Note,
                        keySig = seq.KeySignature.ToString(),
                        timeSig = seq.TimeSignature
                    };

                    noteInfs.Add(newInfo);
                 }

            }

            //calculating note rhythm val: milliseconds for note/480 gives num of quarter beats (~.5 is eighth, ~.25 is sixteenth, we have a bracketing threshold system)
            //array of tuples, iterate thru them and have a resettable counter to keep track of measures 
            //we get one overall key and time
            
            sb.AppendLine($"\\clef {clef}");

            int baseApostrophe = 5;
            int baseComma = 3;
            //' is for middle C or higher (5), and every octave of c above middle C adds another '
            //, is for the C below the C below middle C (3)




            //calculating measures since we have our list of note objects
            var count = 16;
            var num = noteInfs[0].timeSig.Numerator;
            var denom = noteInfs[0].timeSig.Denominator;
            var numSixteenthsLeftInMeasure = count/denom * num;
            //the latest time signature in a string
            string lastTimeSignature = noteInfs[0].timeSig.ToString();
            //the latest key signature in a string 
            string lastKeySignature = noteInfs[0].keySig.ToString();

            //the latest rhythm val of a note
            var latestRhythm = count / (noteInfs[0].length); //gives the lilypond rhythm val
            int i = 0;

            // int currentOctave = int.Parse(noteInfs[0].note[1].ToString());
            foreach(var noteInfo in noteInfs){
                Console.WriteLine(noteInfo.ToString());
                if(string.Compare(noteInfo.timeSig.ToString(), lastKeySignature) != 0){
                    lastKeySignature = noteInfo.timeSig.ToString();

                }
                
                if(string.Compare(noteInfo.timeSig.ToString(), lastTimeSignature) != 0){
                    Console.WriteLine("timesig changed");
                    Console.WriteLine(noteInfo.timeSig.ToString());
                    Console.WriteLine(lastTimeSignature);
                    lastTimeSignature = noteInfo.timeSig.ToString();
                    //begin a new measure 
                    numSixteenthsLeftInMeasure = count/denom * num;
                        //add the current note
                    numSixteenthsLeftInMeasure -= noteInfo.length;
                    sb.Append("\n"); //brand new measure line 
                    sb.Append($"{noteInfo.note[0].ToString().ToLower()}");
                    int currOctave;

                    //there aren't any flats rn
                    if(noteInfo.note.Length == 3){
                        sb.Append("is");
                        //append octave 
                        currOctave = int.Parse(noteInfo.note[2].ToString());
                    } else {
                        //append octave 
                        currOctave = int.Parse(noteInfo.note[1].ToString());
                    }
                                            
                    // int currOctave = int.Parse(noteInfo.note[1].ToString());
                    if(currOctave >= baseApostrophe){
                        while(currOctave >= baseApostrophe){
                            sb.Append("'");
                            currOctave -= 1;
                        }
                    } else {
                        if(currOctave <= baseComma){
                            while(currOctave <= baseComma){
                                sb.Append(",");
                                currOctave += 1;
                            }
                        }
                    }
                    //get Rhythm
                    var currentRhythm = count / noteInfo.length;
                    sb.Append($"{currentRhythm} ");
                    latestRhythm = currentRhythm;

                    //TODO: fill in this logic
                } else {
                    if(numSixteenthsLeftInMeasure >= noteInfo.length){
                        //append note to current measure
                        numSixteenthsLeftInMeasure -= noteInfo.length;
                        sb.Append($"{noteInfo.note[0].ToString().ToLower()}");
                        
                        //there aren't any flats rn
                        int currOctave;
                        if(noteInfo.note.Length == 3){
                            sb.Append("is");
                            //append octave 
                            currOctave = int.Parse(noteInfo.note[2].ToString());
                        } else {
                            //append octave 
                            currOctave = int.Parse(noteInfo.note[1].ToString());
                        }
                        
                        // int currOctave = int.Parse(noteInfo.note[1].ToString());
                        if(currOctave >= baseApostrophe){
                            while(currOctave >= baseApostrophe){
                                sb.Append("'");
                                currOctave -= 1;
                            }
                        } else {
                            if(currOctave <= baseComma){
                                while(currOctave <= baseComma){
                                    sb.Append(",");
                                    currOctave += 1;
                                }
                            }
                        }
                        //get rhythm
                        var currentRhythm = count / noteInfo.length;
                        if(currentRhythm != latestRhythm || i == 0){
                            //initialize for first ever note or update rhythm if it changes
                            sb.Append($"{currentRhythm} ");
                            latestRhythm = currentRhythm;
                        } else {
                            sb.Append(" ");
                        }
                    } else {
                        int currOctave;
                        if(numSixteenthsLeftInMeasure > 0){
                            sb.Append($"{noteInfo.note[0].ToString().ToLower()}");
                            //sharp
                            if(noteInfo.note.Length == 3){
                                sb.Append("is");
                                //append octave 
                                currOctave = int.Parse(noteInfo.note[2].ToString());
                            } else {
                                //append octave 
                                currOctave = int.Parse(noteInfo.note[1].ToString());
                            }
                            //octave
                            if(currOctave >= baseApostrophe){
                                while(currOctave >= baseApostrophe){
                                    sb.Append("'");
                                    currOctave -= 1;
                                }
                            } else {
                                if(currOctave <= baseComma){
                                    while(currOctave <= baseComma){
                                        sb.Append(",");
                                        currOctave += 1;
                                    }
                                }
                            }
                            if((count / numSixteenthsLeftInMeasure) != latestRhythm){
                                var beat = count/numSixteenthsLeftInMeasure;
                                sb.Append($"{beat}");
                            }
                            sb.Append("~");
                        }
                        //adjust length of tied note 
                        var newLength = noteInfo.length-numSixteenthsLeftInMeasure;
                        //new measure, doesn't yet account for tied notes
                        numSixteenthsLeftInMeasure = count/denom * num;
                        //add the current note
                        numSixteenthsLeftInMeasure -= newLength;
                        sb.Append("\n"); //brand new measure line 
                        sb.Append($"{noteInfo.note[0].ToString().ToLower()}");

                        //there aren't any flats rn
                        
                        if(noteInfo.note.Length == 3){
                            sb.Append("is");
                            //append octave 
                            currOctave = int.Parse(noteInfo.note[2].ToString());
                        } else {
                            //append octave 
                            currOctave = int.Parse(noteInfo.note[1].ToString());
                        }
                        if(currOctave >= baseApostrophe){
                            while(currOctave >= baseApostrophe){
                                sb.Append("'");
                                currOctave -= 1;
                            }
                        } else {
                            if(currOctave <= baseComma){
                                while(currOctave <= baseComma){
                                    sb.Append(",");
                                    currOctave += 1;
                                }
                            }
                        }
                        //get Rhythm
                        var currentRhythm = count / newLength;
                        sb.Append($"{currentRhythm} ");
                        latestRhythm = currentRhythm;
   
                    } 
                }
                i += 1;
            }
            sb.AppendLine("}");
            // Console.WriteLine(sb);
            var outPut = $"{fileName}.ly";
            System.IO.File.WriteAllText(@outPut,sb.ToString());
        }
        
    }
}

