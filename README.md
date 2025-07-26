# Salary Algorithm (Pensje XIX IT Olympics)

## Overview
This project implements a salary assignment algorithm for a hierarchical organization, as described in the XIX IT Olympics problem. The program reads a tree of employees (with parent-child relationships) and assigns salaries according to specific rules, ensuring that each subtree receives a unique set of salaries up to a given limit.

## Directory Structure
```
wysepka-salaryalgorythm/
├── README.md
├── Pensje_XIX_IT_Olympics.sln
└── Pensje_XIX_IT_Olympics/
    ├── Pensje_XIX_IT_Olympics.csproj
    └── Program.cs
```

## How to Run
1. **Requirements:**
   - .NET Core 3.1 SDK or newer
2. **Build and Run:**
   - Open a terminal in the `Pensje_XIX_IT_Olympics` directory.
   - Run:
     ```sh
     dotnet run
     ```

## Input Format
- The program expects input from the console (or can be modified to read from a file):
  1. The first line contains an integer `n` — the number of employees.
  2. The next `n` lines each contain two integers: `bossID salary`, where:
     - `bossID` is the 1-based index of the employee's direct superior (or 0 if the employee is the root/boss).
     - `salary` is the employee's salary (0 if not yet assigned).

**Example:**
```
5
0 100
1 0
1 0
2 0
2 0
```

## Output Format
- The program prints the final salaries for all employees, one per line, in the order of input.

**Example Output:**
```
100
99
98
97
96
```

## Algorithm Description
- The algorithm builds a tree from the input, identifies subtrees where salaries need to be assigned, and assigns the highest available salaries to each subtree, ensuring no duplicates and respecting the parent-child constraints.
- The process involves:
  - Parsing input into parent and salary arrays.
  - Identifying subtrees needing salary assignment.
  - Assigning salaries in descending order, ensuring each subtree receives a unique set.
  - Printing the final salary list.

## Files
- `Program.cs`: Main logic for reading input, processing the tree, and assigning salaries.
- `Pensje_XIX_IT_Olympics.csproj`: Project file for .NET Core.
- `Pensje_XIX_IT_Olympics.sln`: Visual Studio solution file.
- `README.md`: (Empty by default, see this file for documentation.)

## Notes
- The program can be easily modified to read input from a file by using the `LoadInitialDataByFile` method in `BFOStream`.
- Debugging methods are available in the code but commented out by default. 
