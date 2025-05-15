using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace msi_GA.GA
{
    public class SaveManagment
    {
        private readonly string _saveDirectory;
        private readonly string _manifestDirectory;


        public string SaveDirectory => _saveDirectory;
        public SaveManagment(string saveDirectory)
        {
            _saveDirectory = saveDirectory;
            _manifestDirectory = Path.Combine(_saveDirectory, "Manifest\\manifest.json");
        }

        public void CheckCreateFolder(int n)
        {
            string basePath = Path.Combine(_saveDirectory, "Saves");
            string folderName = $"{n}Save";
            string fullPath = Path.Combine(basePath, folderName);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
        }

        public void CopyWorkersFile(int generationNumber, string sourceFilePath, string destinationFolderPath)
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException("Plik pracowników nie istnieje", sourceFilePath);
            }

            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }

          
            string destinationFile = Path.Combine(destinationFolderPath, $"generation_{generationNumber}_workers.json");

            try
            {
                File.Copy(sourceFilePath, destinationFile, true);
            }
            catch (IOException ex)
            {
                throw new IOException($"Błąd podczas kopiowania pliku: {ex.Message}", ex);
            }
        }


        public void SaveGeneration(Generation generation)
        {
            CheckCreateFolder(generation.number);

            string savefolderPath = Path.Combine(_saveDirectory, "Saves", $"{generation.number}Save", $"{generation.number}_Generation_{generation.seriesnumber}.json");
            string savepath = Path.Combine(_saveDirectory, "Saves", $"{generation.number}Save");


            CopyWorkersFile(generation.number, generation.filepathWorkers, savepath);

            var dataToSave = new
            {
                GenerationInfo = new
                {
                    generation.number,
                    generation.seriesnumber,
                    generation.ElitistRate,
                    generation.MutationRate,
                    generation.BestParentsKept,
                    generation.GenerationLength,
                },
                ListOfGens = generation._generations.Select(g => new
                {
                    Schedule = g.Schedule,
                    Fitness = g.Fitness,
                    HoursScore = g.HoursScore,
                    ShiftBreakScore = g.ShiftBreakScore,
                    WorkerDispersionScore = g.WorkerDispersionScore,
                    MaxShiftScore = g.MaxShiftScore,
                    TypeOfWorkerPerShift = g.TypeOfWorkerPerShift,
                }).ToList(),
                Statistics = new
                {
                    generation.MaxFitness,
                    generation.MaxEachWorkerTypePerShift,
                    generation.MaxFitnessPerWorker,
                    generation.MaxHoursScore,
                    generation.MaxShiftBreakScore,
                    generation.Mean,
                    generation.Variation
                }
            };

            string json = JsonConvert.SerializeObject(dataToSave, Formatting.Indented);
            File.WriteAllText(savefolderPath, json);
        }

        public Generation LoadGeneration(int generationNumber, int seriesNumber)
        {
            string saveFolderPath = Path.Combine(_saveDirectory, "Saves", $"{generationNumber}Save", $"{generationNumber}_Generation_{seriesNumber}.json");
            string WorkerPath = Path.Combine(_saveDirectory, "Saves", $"{generationNumber}Save", $"generation_{generationNumber}_workers");

            if (!File.Exists(saveFolderPath))
            {
                throw new FileNotFoundException($"Generation file not found at {saveFolderPath}");
            }



            string json = File.ReadAllText(saveFolderPath);
            // Use anonymous type for deserialization
            var data = JsonConvert.DeserializeAnonymousType(json, new
            {
                GenerationInfo = new
                {
                    number = 0,
                    seriesnumber = 0,
                    ElitistRate = 0,
                    MutationRate = 0.0,
                    BestParentsKept = 0,
                    GenerationLength = 0,
                },
                ListOfGens = new[] { new
            {
                Schedule = new int[][]{ new int[]{0}} ,
                Fitness = 0,
                HoursScore = 0,
                ShiftBreakScore = 0,
                WorkerDispersionScore = 0,
                MaxShiftScore = 0,
                TypeOfWorkerPerShift = 0,
            } }.ToList(),
                Statistics = new
                {
                    MaxFitness = 0,
                    MaxEachWorkerTypePerShift = 0,
                    MaxFitnessPerWorker = 0.0,
                    MaxHoursScore = 0,
                    MaxShiftBreakScore = 0,
                    Mean = 0.0,
                    Variation = 0.0
                }
            });

            if (data == null)
            {
                throw new JsonException("Failed to deserialize generation data. The file may be empty or invalid.");
            }
            Generation generation = new Generation ()
            {
                number = data.GenerationInfo.number,
                seriesnumber = data.GenerationInfo.seriesnumber,
                ElitistRate = data.GenerationInfo.ElitistRate,
                MutationRate = data.GenerationInfo.MutationRate,
                BestParentsKept = data.GenerationInfo.BestParentsKept,
                GenerationLength = data.GenerationInfo.GenerationLength,
                _generations = data.ListOfGens.Select(g => new Gen
                {
                    Schedule = g.Schedule,
                    Fitness = g.Fitness,
                    HoursScore = g.HoursScore,
                    ShiftBreakScore = g.ShiftBreakScore,
                    WorkerDispersionScore = g.WorkerDispersionScore,
                    MaxShiftScore = g.MaxShiftScore,
                    TypeOfWorkerPerShift = g.TypeOfWorkerPerShift
                }).ToList(),
                MaxFitness = data.Statistics.MaxFitness,
                MaxEachWorkerTypePerShift = data.Statistics.MaxEachWorkerTypePerShift,
                MaxFitnessPerWorker = data.Statistics.MaxFitnessPerWorker,
                MaxHoursScore = data.Statistics.MaxHoursScore,
                MaxShiftBreakScore = data.Statistics.MaxShiftBreakScore,
                Mean = data.Statistics.Mean,
                Variation = data.Statistics.Variation
            };

            string workerFilePath = Path.Combine(_saveDirectory, "Saves", $"{generation.number}Save", $"generation_{generation.number}_workers.json");
            generation.LoadWorkersFromJson(workerFilePath);

            return generation;
        }



     
       

        public void CreateEmptyManifest()
        {
          

            var manifest = new
            {
                LastCreatedFileId = 0,
            };

            string json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            File.WriteAllText(_manifestDirectory, json);
        }

        public void UpdateManifest(int number) 
        {
            

            try
            {
                string json = File.ReadAllText(_manifestDirectory);
                var manifest = JsonConvert.DeserializeObject<ManifestModel>(json);
                if (manifest != null)
                {
                    manifest.LastCreatedFileId = number; 
                    string updatedJson = JsonConvert.SerializeObject(manifest, Formatting.Indented);
                    File.WriteAllText(_manifestDirectory, updatedJson); 
                }
                else
                {
                    
                    throw new InvalidOperationException("Manifest file was empty or invalid.");
                }
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Failed to parse manifest file", ex);
            }
            catch (IOException ex)
            {
                throw new IOException("Failed to write to manifest file", ex);
            }

        }


        public int ReadManifest()
        {
            

            if (!File.Exists(_manifestDirectory))
            {
                CreateEmptyManifest();
            }

            try
            {
                string json = File.ReadAllText(_manifestDirectory);
                var manifest = JsonConvert.DeserializeObject<ManifestModel>(json);
                return manifest?.LastCreatedFileId ?? 0;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Failed to parse manifest file", ex);
            }
        }
    }

    public class ManifestModel
    {
        public int LastCreatedFileId { get; set; }
    }
}
