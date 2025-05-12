# Genetic Algorithm for Staff Scheduling in a Power Plant (still developed)

This project implements a **Genetic Algorithm (GA)** to solve the **staff scheduling problem** for a power plant. The goal of the algorithm is to create an optimal schedule for plant staff, ensuring coverage while considering constraints such as work shifts, maximum working hours, and required skills for each shift.

## Problem Overview

In a power plant, staff scheduling is a critical task to ensure efficient operation while adhering to labor laws and shift preferences. The scheduling process must account for:
- The number of staff needed during different shifts.
- Constraints on maximum working hours.
- Ensuring staff has the required skills for each shift.
- Balancing work-life preferences and minimizing staff dissatisfaction.

The Genetic Algorithm aims to find an optimal or near-optimal solution by evolving a population of possible schedules over generations.

## Key Features
- **Population Initialization**: Randomly generates an initial population of potential schedules.
- **Fitness Function**: Evaluates each schedule based on how well it satisfies constraints like working hours, required skills, and coverage.
- **Selection**: Selects the best schedules for reproduction based on their fitness.
- **Crossover**: Combines parts of two parent schedules to create new schedules.
- **Mutation**: Introduces small random changes to schedules to promote diversity and prevent premature convergence.
- **Termination Condition**: The algorithm stops when a satisfactory schedule is found or after a set number of generations.



