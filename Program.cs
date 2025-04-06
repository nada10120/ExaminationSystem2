using System;
using System.Collections.Generic;
using System.IO;

namespace Examination_system
{
    enum ExamMode
    {
        Starting,
        Queued,
        Finished
    }

    // Base class for questions
    class Question
    {
        public string Header { get; set; }
        public string Body { get; set; }
        public int Mark { get; set; }
        public AnswerList Answers { get; set; }

        public Question(string header, string body, int mark)
        {
            Header = header;
            Body = body;
            Mark = mark;
            Answers = new AnswerList();
        }
    }

    // Derived class for single-choice questions
    class ChooseOneQuestion : Question
    {
        public string[] Options { get; set; }
        public int CorrectIndex { get; set; }

        public ChooseOneQuestion(string header, string body, int mark, string[] options, int correctIndex)
            : base(header, body, mark)
        {
            Options = options;
            CorrectIndex = correctIndex;
            for (int i = 0; i < options.Length; i++)
            {
                Answers.Add(new Answer(options[i]) { IsCorrect = (i == correctIndex) });
            }
        }
    }

    // Class representing an answer
    class Answer
    {
        public string questionAnswer { get; set; }
        public bool IsCorrect { get; set; }

        public Answer(string answer)
        {
            questionAnswer = answer;
        }
    }

    // List of answers
    class AnswerList : List<Answer>
    {
        public AnswerList()
        {
        }
    }

    // List of questions with file logging
    class QuestionList : List<Question>
    {
        public string listName { get; set; }
        private string filePath { get; set; }

        public QuestionList(string ln, string basePath = "C:\\Exams")
        {
            listName = ln;
            filePath = Path.Combine(basePath, $"{listName}.txt");
        }

        public new bool Add(Question que)
        {
            base.Add(que);
            return true;
        }
    }

    // Abstract base class for exams with file operations
    abstract class Exam
    {
        public TimeSpan Duration { get; set; }
        public int NumberOfQuestions => Questions.Count;
        public Dictionary<Question, AnswerList> QuestionAnswers { get; set; }
        public QuestionList Questions { get; set; }
        public ExamMode Mode { get; set; }
        public event EventHandler ExamStarted;
        public string ExamFilePath { get; set; }

        protected Exam(TimeSpan duration, string logFile)
        {
            Duration = duration;
            Questions = new QuestionList(logFile);
            QuestionAnswers = new Dictionary<Question, AnswerList>();
            Mode = ExamMode.Queued;
            ExamFilePath = Path.Combine("C:\\Exams", $"{logFile}.txt");
            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(ExamFilePath));
        }

        public virtual void Start()
        {
            Mode = ExamMode.Starting;
            ExamStarted?.Invoke(this, EventArgs.Empty);
        }

        public abstract void ShowExam();

        // Save exam details to file
        public void SaveExamToFile()
        {
            using (StreamWriter sw = new StreamWriter(ExamFilePath))
            {
                sw.WriteLine($"Duration: {Duration.TotalMinutes}");
                sw.WriteLine($"NumberOfQuestions: {NumberOfQuestions}");
                foreach (var question in Questions)
                {
                    sw.WriteLine($"Header: {question.Header}");
                    sw.WriteLine($"Body: {question.Body}");
                    sw.WriteLine($"Mark: {question.Mark}");
                    sw.WriteLine($"Options: {string.Join("|", (question as ChooseOneQuestion).Options)}");
                    sw.WriteLine($"CorrectIndex: {(question as ChooseOneQuestion).CorrectIndex}");
                    sw.WriteLine();
                }
            }
            Console.WriteLine($"Exam saved to {ExamFilePath}");
        }

        // Load exam details from file
        public void LoadExamFromFile()
        {
            if (!File.Exists(ExamFilePath))
            {
                Console.WriteLine($"No exam file found at {ExamFilePath}");
                return;
            }

            Questions.Clear();
            var lines = File.ReadAllLines(ExamFilePath);
            int i = 0;

            Duration = TimeSpan.FromMinutes(double.Parse(lines[i].Split(':')[1].Trim()));
            i++;
            int numQuestions = int.Parse(lines[i].Split(':')[1].Trim());
            i++;

            for (int q = 0; q < numQuestions; q++)
            {
                string header = lines[i].Split(':')[1].Trim(); i++;
                string body = lines[i].Split(':')[1].Trim(); i++;
                int mark = int.Parse(lines[i].Split(':')[1].Trim()); i++;
                string[] options = lines[i].Split(':')[1].Trim().Split('|'); i++;
                int correctIndex = int.Parse(lines[i].Split(':')[1].Trim()); i++;
                i++; // Skip empty line

                var question = new ChooseOneQuestion(header, body, mark, options, correctIndex);
                Questions.Add(question);
            }
        }
    }

    // Practice exam implementation
    class PracticeExam : Exam
    {
        public PracticeExam(TimeSpan duration, string logFile)
            : base(duration, logFile)
        {
        }

        public override void ShowExam()
        {
            LoadExamFromFile();
            Console.WriteLine("Practice Exam:");
            Console.WriteLine($"Duration: {Duration.TotalMinutes} minutes");
            Console.WriteLine($"Number of Questions: {NumberOfQuestions}");
            foreach (var question in Questions)
            {
                Console.WriteLine($"Header: {question.Header}");
                Console.WriteLine($"Body: {question.Body}");
                Console.WriteLine($"Mark: {question.Mark}");
                var options = (question as ChooseOneQuestion).Options;
                for (int i = 0; i < options.Length; i++)
                {
                    Console.WriteLine($"Option {i + 1}: {options[i]} {(i == (question as ChooseOneQuestion).CorrectIndex ? "(Correct)" : "")}");
                }
                Console.WriteLine();
            }
            Mode = ExamMode.Finished;
        }
    }

    // Final exam implementation
    class FinalExam : Exam
    {
        public FinalExam(TimeSpan duration, string logFile)
            : base(duration, logFile)
        {
        }

        public override void ShowExam()
        {
            LoadExamFromFile();
            Console.WriteLine("Final Exam:");
            Console.WriteLine($"Duration: {Duration.TotalMinutes} minutes");
            Console.WriteLine($"Number of Questions: {NumberOfQuestions}");
            foreach (var question in Questions)
            {
                Console.WriteLine($"Header: {question.Header}");
                Console.WriteLine($"Body: {question.Body}");
                Console.WriteLine($"Mark: {question.Mark}");
                var options = (question as ChooseOneQuestion).Options;
                for (int i = 0; i < options.Length; i++)
                {
                    Console.WriteLine($"Option {i + 1}: {options[i]}");
                }
                Console.WriteLine();
            }
            Mode = ExamMode.Finished;
        }
    }

    // Main program
    internal class Program
    {
        static void Main(string[] args)
        {
            bool cont = true;
            while (cont)
            {
                Console.WriteLine("Good Morning!\nWho are you today?\n1- Student\n2- Teacher\nPlease choose a number to identify:");
                int choice = int.Parse(Console.ReadLine());
                if (choice == 1)
                {
                    Console.WriteLine("Welcome student!\nWhat exam would you like to take today?\n1- Practice\n2- Final");
                    int examChoice = int.Parse(Console.ReadLine());
                    if (examChoice == 1)
                    {
                        Console.WriteLine("Enter the exam name to load:");
                        string examName = Console.ReadLine();
                        var practiceExam = new PracticeExam(TimeSpan.FromMinutes(30), examName);
                        practiceExam.ExamStarted += (sender, e) => Console.WriteLine("Exam has started!");
                        practiceExam.Start();
                        practiceExam.ShowExam();
                    }
                    else if (examChoice == 2)
                    {
                        Console.WriteLine("Enter the exam name to load:");
                        string examName = Console.ReadLine();
                        var finalExam = new FinalExam(TimeSpan.FromMinutes(60), examName);
                        finalExam.ExamStarted += (sender, e) => Console.WriteLine("Exam has started!");
                        finalExam.Start();
                        finalExam.ShowExam();
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice, please try again.");
                    }
                }
                else if (choice == 2)
                {
                    Console.WriteLine("Welcome teacher");
                    Console.WriteLine("Would you like to create a practice exam or final exam?\n(y)-Practice\n(n)-Final\nChoose a specified letter:");
                    string createExam = Console.ReadLine().ToLower();
                    Exam exam = null;
                    if (createExam == "y" || createExam == "n")
                    {
                        Console.WriteLine("Creating an exam...");
                        Console.WriteLine("Enter the exam name:");
                        string examName = Console.ReadLine();
                        Console.WriteLine("Enter the exam duration (in minutes):");
                        double duration = double.Parse(Console.ReadLine());

                        Console.WriteLine("Enter the number of questions:");
                        int numberOfQuestions = int.Parse(Console.ReadLine());

                        exam = createExam == "y"
                            ? new PracticeExam(TimeSpan.FromMinutes(duration), examName)
                            : new FinalExam(TimeSpan.FromMinutes(duration), examName);

                        for (int i = 0; i < numberOfQuestions; i++)
                        {
                            Console.WriteLine($"Enter question {i + 1} header:");
                            string header = Console.ReadLine();
                            Console.WriteLine($"Enter question {i + 1} body:");
                            string body = Console.ReadLine();
                            Console.WriteLine($"Enter question {i + 1} mark:");
                            int mark = int.Parse(Console.ReadLine());
                            Console.WriteLine($"Enter the number of options for question {i + 1}:");
                            int optionCount = int.Parse(Console.ReadLine());
                            string[] options = new string[optionCount];
                            for (int j = 0; j < optionCount; j++)
                            {
                                Console.WriteLine($"Enter option {j + 1}:");
                                options[j] = Console.ReadLine();
                            }
                            Console.WriteLine("Enter the index of the correct answer (0-based):");
                            int correctIndex = int.Parse(Console.ReadLine());
                            var question = new ChooseOneQuestion(header, body, mark, options, correctIndex);
                            exam.Questions.Add(question);
                        }
                        exam.SaveExamToFile();
                        Console.WriteLine("Exam created and saved successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice, please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice, please try again.");
                }
                Console.WriteLine("Continue? (y/n)");
                cont = Console.ReadLine().ToLower() == "y";
            }
        }
    }
}