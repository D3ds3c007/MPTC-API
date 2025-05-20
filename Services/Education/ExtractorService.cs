using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MPTC_API.Models.Education;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Text.RegularExpressions;
using Subject = MPTC_API.Services.Education.UtilClasses.Subject;
using Section = MPTC_API.Services.Education.UtilClasses.Section;
using Question = MPTC_API.Services.Education.UtilClasses.Question;
using AssetNote = MPTC_API.Services.Education.UtilClasses.AssetNote;

namespace MPTC_API.Services.Education
{
    public class ExtractorService
    {
        //Function to extract text from a PDF file
        public static string extractPDF(string pdfPath)
        {
            string allText = "";

            // Open the PDF file
            using (PdfReader pdfReader = new PdfReader(pdfPath))
            using (PdfDocument pdfDoc = new PdfDocument(pdfReader))
            {
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    // Extract text from each page
                    string text = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i));

                    // Append the text with a newline after each page
                    allText += text + "\n"; // Add a newline after each page's content
                }
            }

            // Console.WriteLine(allText);
            return allText.Trim();
        }

        //Function to clean Subject
        // Remove lines that start with "Page" or "Matricule"
        public static string cleanPDF(string subjectPdfText)
        {
            string cleanedText = Regex.Replace(subjectPdfText, @"(?m)^\s*(Page|Matricule).*", "", RegexOptions.IgnoreCase);
            // Console.WriteLine(cleanedText);
            return cleanedText;
        }

        //Function create Subject string
        public static Subject createSubject(string subjectPdfText)
        {
            Console.WriteLine("Test create Subject start here");

            Subject subject = new Subject();

            // section with points
            string sectionPattern = @"(?<section>(GRAMMAR|READING COMPREHENSION|VOCABULARY|WRITING|LISTENING))\s*\[\s*Total\s*:\s*(?<points>\d+)\s*points\s*\](?<content>[\s\S]*?)(?=(GRAMMAR|READING COMPREHENSION|VOCABULARY|WRITING|LISTENING)|\Z)";

            var sectionMatches = Regex.Matches(subjectPdfText, sectionPattern, RegexOptions.Singleline);
            int sectionCount = 1;

            foreach (Match sectionMatch in sectionMatches)
            {
                string sectionName = sectionMatch.Groups["section"].Value.Trim();
                string sectionContent = sectionMatch.Groups["content"].Value.Trim();
                double sectionPoints = Double.Parse(sectionMatch.Groups["points"].Value.Trim());

                Section section = new Section();
                section.Name = sectionName;
                section.Scale = sectionPoints;

                // Console.WriteLine($"Section {sectionCount}: {section.Name} {section.Scale}");
                // Console.WriteLine($"Content : {sectionContent}");

                string questionPattern = @"(?<number>\d+)\.\s*(?<question>[\s\S]+?)\s*\(\s*(?<points>\d+)\s*points\s*\)(?<content>[\s\S]*?)(?=\n\s*\d+\.\s*|\Z)";

                var questionMatches = Regex.Matches(sectionContent, questionPattern, RegexOptions.Singleline);
                int expectedQuestionNumber = 1;

                foreach (Match questionMatch in questionMatches)
                {
                    // Console.WriteLine($"Question Match : {questionMatch}");

                    int actualQuestionNumber = int.Parse(questionMatch.Groups["number"].Value.Split('.')[0]);
                    if (actualQuestionNumber != expectedQuestionNumber)
                    {
                        Console.WriteLine($"Skipping invalid question number: {actualQuestionNumber} (Expected: {expectedQuestionNumber})");
                        continue; // Ignore out-of-sequence questions
                    }

                    // string questionText = questionMatch.Value.Trim();
                    string questionPoints = questionMatch.Groups["points"].Value;
                    string content = questionMatch.Groups["question"].Value.Trim();
                    string questionContent = questionMatch.Groups["content"].Value.Trim();

                    // Ensure multi-line question text is properly formatted
                    string formattedContent = Regex.Replace(content, @"\s*\n\s*", " "); // Replace line breaks with space

                    Question q = new Question();
                    q.Number = expectedQuestionNumber;
                    q.Text = formattedContent; // Now supports multi-line questions
                    q.Scale = Double.Parse(questionPoints);

                    // Console.WriteLine($"Question: {q.Name}: {q.Text} {q.Scale}");
                    // Console.WriteLine($"questionText : {questionText}");
                    // Console.WriteLine($"content : {content}");
                    // Console.WriteLine($"questionContent : {questionContent}");
                    // Console.WriteLine($"formattedContent : {formattedContent}");


                    string firstAnswerPattern = @"^[\s\S]*?(?=\na[\.\)\s])";

                    // Use Regex.Replace to remove everything before the first "a)" answer
                    string allItems = Regex.Replace(formattedContent, firstAnswerPattern, "");
                    // Console.WriteLine($" Content : {questionContent}: ");

                    if (section.Name == "WRITING")
                    {
                        q.Items.Add("a", questionContent);
                    }
                    else
                    {

                        string[] lines = questionContent.Split('\n');

                        // Start from letter 'a' for the answers
                        char expectedLetter = 'a';
                        string lastKey = null;
                        string key = null;
                        string value = null;

                        foreach (string line in lines)
                        {
                            string trimmedLine = line.Trim();
                            // Console.WriteLine($"trimmedLine: {trimmedLine}");

                            // Check if the line starts with the expected letter followed by ')', '.', or space
                            if (trimmedLine.StartsWith($"{expectedLetter})") || trimmedLine.StartsWith($"{expectedLetter}."))
                            {
                                // If the line starts with the expected letter, extract the key and value
                                key = expectedLetter.ToString();
                                value = trimmedLine.Substring(2).Trim();  // Extract value after key

                                // Add the key-value pair to the dictionary (or update it)
                                q.Items.Add(key, value);

                                // Move to the next letter in the ASCII table
                                expectedLetter++;
                                lastKey = key;  // Update the last key to the current one
                                // Console.WriteLine($"key: {key}, value: {value}");
                                // Console.WriteLine($"lastKey: {lastKey}");

                            }
                            else if (lastKey != null)
                            {
                                // If the line doesn't match the expected letter, append it to the last key's value
                                q.Items[lastKey] += " " + trimmedLine;  // Append with a space
                                // Console.WriteLine($"lastKey: {lastKey}, value: {q.Items[lastKey]}");
                            }
                        }
                    }

                    section.Questions.Add(q);
                    expectedQuestionNumber++;
                }

                subject.Sections.Add(section);
                sectionCount++;
            }

            Console.WriteLine("Test create Subject end here");

            // foreach (var section in subject.Sections){
            //     Console.WriteLine($"📌 Section: {section.Name}");
            //     foreach (var question in section.Questions){
            //         Console.WriteLine($"   ❓ {question.Name}:");
            //         foreach (var item in question.Items){
            //             Console.WriteLine($"      🔹 {item.Key}) {item.Value}");
            //         }
            //     }
            //     Console.WriteLine(); // Add spacing for readability
            // }

            return subject;
        }

        //Function create StudentAnswer string
        public static AssetNote createAssetNote(string inputText)
        {
            Console.WriteLine("Test create AssetNote start here");

            AssetNote assetnote = new AssetNote();

            string sectionPattern = @"(?<section>(GRAMMAR|READING COMPREHENSION|VOCABULARY|WRITING|LISTENING))\s*\((?<points>\d+)\)[\s\S]*?(?=(GRAMMAR|READING COMPREHENSION|VOCABULARY|WRITING|LISTENING)|\Z)";

            var sectionMatches = Regex.Matches(inputText, sectionPattern, RegexOptions.Singleline);
            int sectionCount = 1;

            foreach (Match sectionMatch in sectionMatches)
            {
                string sectionName = sectionMatch.Groups["section"].Value.Trim();
                string sectionPoints = sectionMatch.Groups["points"].Value.Trim();
                string sectionContent = sectionMatch.Value.Trim();

                Section section = new Section();
                section.Name = sectionName;
                section.Scale = Double.Parse(sectionPoints);

                // Console.WriteLine($"\nSection {sectionCount}: {section.Name} {section.Scale}");
                // Console.WriteLine($"\nsectionContent {sectionContent}" );

                //update questionPattern to include an optional type capture
                // string questionPattern = @"(?<question>Question\s*\d+\s*\((?<points>\d+)\)\s*(?:\[(?<type>\d+)\])?\s*(Description\s*:\s*(?<description>.*?)\n)?[\s\S]*?)(?=\n\s*Question\s*\d+\s*\(\d+\)\s*|$)";

                // questionpattern with optional content            
                // string questionPattern = @"(?<question>Question\s*\d+\s*\((?<points>\d+)\)\s*(?:\[(?<type>\d+)\])?\s*(?:Description\s*:\s*(?<description>.*?)\n)?)(?<content>(?:(?!\n\s*Question\s*\d+\s*\(\d+\)).)*?)";

                //questionpattern with optional content until the next question begins or until no more content is present
                // string questionPattern = @"(?<question>Question\s*\d+\s*\((?<points>\d+)\)\s*(?:\[(?<type>\d+)\])?\s*(?:Description\s*:\s*(?<description>.*?)\n)?)(?<content>(?:(?:(?!\n\s*Question\s*\d+\s*\(\d+\)).|\n)*)?)";
                string questionPattern = @"(?<question>Question\s*\d+\s*\((?<points>\d+)\)\s*(?:Description\s*:\s*(?<description>.*?)\n)?)(?<content>(?:(?:(?!\n\s*Question\s*\d+\s*\(\d+\)).|\n)*)?)";

                var questionMatches = Regex.Matches(sectionContent, questionPattern, RegexOptions.Singleline);
                // Console.WriteLine($"Number of questions: {questionMatches.Count}");

                int questionCount = 1;

                foreach (Match questionMatch in questionMatches)
                {
                    string questionText = questionMatch.Value.Trim();
                    string questionPoints = questionMatch.Groups["points"].Value; // The captured points
                    string questionDescription = questionMatch.Groups["description"].Value.Trim(); // Capture the description if present
                    string type = questionMatch.Groups["type"].Value;
                    string content = questionMatch.Groups["content"].Value.Trim(); // Capture the description if present

                    Question q = new Question();
                    q.Number = questionCount;
                    q.Scale = Double.Parse(questionPoints);
                    q.Description = questionDescription;

                    // Console.WriteLine($"Question: {q.Name}: {q.Scale} {q.Type} {q.Description} ");
                    // Console.WriteLine($"Content: {questionText}");

                    // Define a regex pattern to remove everything before the first answer pattern
                    string fisrtAnswerPattern = @"^[\s\S]*?(?=\na[\.\)\s])";

                    bool hasFirstAnswer = Regex.IsMatch(questionText, fisrtAnswerPattern);

                    if (hasFirstAnswer)
                    {
                        // Console.WriteLine("hasFirstAnswer");

                        // Use Regex.Replace to isolate the question content after the first "a)"
                        string questionContent = Regex.Replace(questionText, fisrtAnswerPattern, "");

                        string[] lines = questionContent.Split('\n');
                        char expectedLetter = 'a';
                        string lastKey = null;

                        foreach (string line in lines)
                        {
                            string trimmedLine = line.Trim();

                            if (trimmedLine.StartsWith($"{expectedLetter})"))
                            {
                                string key = expectedLetter.ToString();
                                string value = trimmedLine.Substring(2).Trim();

                                q.Items.Add(key, value);

                                expectedLetter++;
                                lastKey = key;
                                // Console.WriteLine($"key: {key}, value: {value}");
                                // Console.WriteLine($"lastKey: {lastKey}");

                            }
                            else if (lastKey != null)
                            {
                                q.Items[lastKey] += " " + trimmedLine;
                                // Console.WriteLine($"lastKey: {lastKey}, actual value: {q.Items[lastKey]}");
                            }
                        }
                    }
                    else
                    {
                        // Console.WriteLine("Not hasFirstAnswer");

                        // If no first answer pattern is found, indicate that the question has no expected answers
                        q.Items["N/A"] = "No expected answers provided.";
                        // Console.WriteLine($"Key: N/A, Value: {q.Items["N/A"]}");

                    }
                    section.Questions.Add(q);
                    questionCount++;
                }
                assetnote.Sections.Add(section);
                sectionCount++;
            }

            Console.WriteLine("Test create AssetNote end here");

            return assetnote;
        }

        // Function to combine Subject and Assetnote
        public static List<string[]> combineSubjectAndAssetnote(Subject subject, AssetNote assetnote)
        {
            Console.WriteLine("Test combine subject and assetnote start here");

            List<string[]> fullnotes = new List<string[]>{
                new string[] { "SectionName", "SectionScale", "QuestionNumber", "QuestionText", "QuestionScale", "QuestionDescription","QuestionCorrectType", "ItemNumber", "ItemAnswer" },
            };

            // Combine the sections of the two objects
            foreach (Section section in subject.Sections)
            {

                // Check if the section already exists in the assetnote
                Section existingSection = assetnote.Sections.FirstOrDefault(s => s.Name == section.Name);
                fullnotes.Add(new string[] { section.Name, existingSection.Scale.ToString(), "", "", "", "", "", "", "" });

                // Console.WriteLine($"Section: {section.Name}");

                if (existingSection != null)
                {
                    // Combine questions from both sections
                    foreach (Question question in section.Questions)
                    {
                        int correctType = defineQuestionCorrectionType(question.Text);

                        // Console.WriteLine($"Question {question.Number} - correcttype : {correctType}");

                        fullnotes.Add(new string[] { "", "", question.Number.ToString(), question.Text, question.Scale.ToString(), question.Description, correctType.ToString(), "", "" });

                        Question existingQuestion = existingSection.Questions.FirstOrDefault(q => q.Number == question.Number);

                        if (existingQuestion != null)
                        {
                            if (existingQuestion.Items.Count > 0)
                            {

                                foreach (var item in existingQuestion.Items)
                                {
                                    string itemNumber = item.Key;
                                    string itemAnswer = item.Value;
                                    fullnotes.Add(new string[] { "", "", "", "", "", "", "", itemNumber, itemAnswer });
                                }
                            }
                        }
                    }
                }
            }

            // foreach (var row in fullnotes)
            // {
            //     Console.WriteLine(string.Join(" | ", row));
            // }

            Console.WriteLine("Test combine subject and assetnote end here");

            return fullnotes;
        }

        // Function define question type
        public static int defineQuestionCorrectionType(string questionText)
        {
            int correctType = 0;
            string[][] correctiontypes = new string[][]{
                ["choose","match","complete","fill","picture"],
                ["answer", "true or false"],
                ["write","create"]
            };

            bool found = false;

            for (int i = 0; i < correctiontypes.Length && !found; i++)
            {
                for (int j = 0; j < correctiontypes[i].Length; j++)
                {

                    string phrase = correctiontypes[i][j];
                    string pattern = $@"\b{Regex.Escape(phrase)}\b";
                    bool containsPhrase = Regex.IsMatch(questionText, pattern, RegexOptions.IgnoreCase);

                    if (containsPhrase)
                    {
                        // Console.WriteLine($"The sentence contains the word {phrase}.");
                        correctType = i;
                        found = true;
                        break; // Exit the inner loop if a match is found
                    }
                }
            }

            return correctType + 1;
        }

        // Function to write on csv
        public static void writeOnCsv(string filePath, List<string[]> fullnotes)
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                foreach (var row in fullnotes)
                {
                    string line = string.Join(",", row);
                    writer.WriteLine(line);
                }
            }
            Console.WriteLine("Fichier CSV généré avec succès !");
        }

        public static Subject extarctSubjectText(string subjectPath)
        {
            string subjectText = extractPDF(subjectPath);
            string cleanedText = cleanPDF(subjectText);
            Subject subject = createSubject(cleanedText);
            return subject;
        }

        public static AssetNote extarctAssetNoteText(string assetNotePath)
        {
            string AssetNoteText = extractPDF(assetNotePath);
            string cleanedText = cleanPDF(AssetNoteText);
            AssetNote assetNote = createAssetNote(cleanedText);
            return assetNote;
        }

        public static void createCSV(string subjectPath, string assetNotePath, string examName)
        {

            Subject subject = extarctSubjectText(subjectPath);
            AssetNote assetNote = extarctAssetNoteText(assetNotePath);
            List<string[]> fullnotes = combineSubjectAndAssetnote(subject, assetNote);

            string csvPath = Path.Combine("D:\\Mobile\\MPTC-API\\Temp\\", $"{examName}.csv");
            writeOnCsv(csvPath, fullnotes);

            Console.WriteLine("CSV file created successfully!");
        }






    }

}
