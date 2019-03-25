using System;
using System.Dynamic;
using System.IO;

namespace CSVSplitter
{
    class Program
    {
        static void Main(string[] args)
        {
            // Display the intro line.
            Console.WriteLine("Welcome to CSV splitter.");
            
            // Get the path to the file
            var filePath = "";
            while (!File.Exists(filePath))
            {
                Console.WriteLine("Please enter the path and file name of your CSV file: ");
                filePath = Console.ReadLine();
            }

            // Get the max rows per file.
            var maxRowsString = "";
            int maxRows;
            while (!int.TryParse(maxRowsString, out maxRows))
            {
                Console.WriteLine("Please enter the maximum number of rows per file: ");
                maxRowsString = Console.ReadLine();   
            }

            // Use the first row in the source file as the first row for each file.
            var headerRowString = "";
            while (headerRowString == null || (headerRowString.ToUpper() != "Y" && headerRowString.ToUpper() != "N"))
            {
                Console.WriteLine("Use the first row of the source file as the first row of each file? Y/N");
                headerRowString = Console.ReadLine();
            }

            var headerRow = headerRowString.ToUpper() == "Y";
            
            // Present the start message.
            var withHeaderMessage = headerRow ? " Using the first row of the source file as the first row for each split file." : "";
            Console.WriteLine("Splitting " + filePath + " into files with a maximum of " + maxRows + " rows." + withHeaderMessage);
            
            var filesCounter = 0;
            
            // Load the file.
            var sourceFile = new FileStream(filePath, FileMode.Open);
            using (var reader = new StreamReader(sourceFile))
            {
                var isFirstLine = true;
                var firstLine = "";
                var fileLines = 0;
                
                // Get the source file name to create the split file names.
                var sourceFileName = Path.GetFileNameWithoutExtension(filePath);
                var sourceFileExt = Path.GetExtension(filePath);
                
                // Get a handler for the file to write.
                FileStream outFile = null;
                StreamWriter fileWriter = null;
                
                // Loop until out of rows in the source file.
                string line;
                while ((line = reader.ReadLine()) != null) 
                {
                    // Get the header if necessary.
                    if (isFirstLine && headerRow)
                    {
                        firstLine = line;
                        isFirstLine = false;
                    }

                    if (fileLines == 0)
                    {
                        // Open a new write file.
                        outFile = new FileStream(sourceFileName + "_split_" + "filesCounter" + sourceFileExt, FileMode.CreateNew);
                        fileWriter = new StreamWriter(outFile);
                        // Increment the file count.
                        filesCounter++;
                        // Write the first line and increment the count if applicable.
                        if (headerRow)
                        {
                            fileWriter.WriteLine(firstLine);
                            fileLines++;
                        }
                    }
                    
                    // Write the line
                    if (fileWriter != null)
                    {
                        fileWriter.WriteLine(line);
                    }

                    // Increment the count again, if it's reached the max, close the current file and reset the file Lines.
                    fileLines++;
                    if (fileLines >= maxRows)
                    {
                        if (fileWriter != null)
                        {
                            fileWriter.Dispose();
                        }

                        if (outFile != null)
                        {
                            outFile.Close();
                        }
                        fileLines = 0;
                    }
                }

                if (fileWriter != null)
                {
                    fileWriter.Dispose();
                }
                
                if (outFile != null)
                {
                    outFile.Close();
                }
            }
            sourceFile.Close();
            // Display completion message.
            Console.WriteLine("Split " + filePath + " into " + filesCounter + " files.");
        }
    }
}