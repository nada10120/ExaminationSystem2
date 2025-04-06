# Examination System

A simple examination system built with C# and .NET, allowing teachers to create multiple-choice exams and students to take them. Exams are saved to and loaded from text files for persistence.

## Features

- Teachers can create practice or final exams with multiple-choice questions.
- Exams are saved to text files for later retrieval.
- Students can load and take exams, with practice exams showing correct answers.
- Basic console interface for interaction.

## Technologies Used

- C# 
- Grok (for coding assistance)

## Setup Instructions

1. **Clone the Repository**:  
   ```bash
   git clone https://github.com/your-username/ExaminationSystem.git
   ```
2. **Navigate to the Project Directory**:  
   ```bash
   cd ExaminationSystem
   ```
3. **Build the Project**:  
   Open the solution in Visual Studio and build it, or use the .NET CLI:
   ```bash
   dotnet build
   ```
4. **Run the Application**:  
   ```bash
   dotnet run
   ```

## Usage

1. **Teacher Mode**:  
   - Choose option 2 to enter teacher mode.
   - Create a practice or final exam by providing details like exam name, duration, and questions.
   - Exams are saved to `C:\Exams\{examName}.txt`.

2. **Student Mode**:  
   - Choose option 1 to enter student mode.
   - Select the exam type and enter the exam name to load and display the exam.

## File Structure

- `ExaminationSystem.cs`: Main source file containing all classes and logic.
- `C:\Exams\`: Directory where exam files are saved (adjustable in code).

## Contribution

Feel free to fork the repository and submit pull requests with improvements or bug fixes.


## Contact

For questions or feedback, reach out to me at [nadayahia410@gmail.com].
