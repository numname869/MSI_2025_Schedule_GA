using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace msi_GA.GA
{
    public class SaveManagment
    {

        private readonly string _saveDirectory;
        private readonly string _manifestDirectory;
      

        public SaveManagment(string saveDirectory)
        {
              _saveDirectory = saveDirectory;
            _manifestDirectory = Path.Combine(_saveDirectory, "\\Manifest");
        }


     


        public void SaveGeneration(Generation generation)
        {
            string savePath = Path.Combine(_saveDirectory, $"{generation.number}_Generation_{generation.seriesnumber}.json");
            string SaveWorkersPath =generation.filepathWorkers ;

            var dataToSave = new
            {
                //dokonczyc pozniej
                GenerationInfo = new
                {
                    generation.number,
                    generation.seriesnumber,
                    generation.ElitistRate,
                    generation.MutationRate
                    generation.GenerationLength
                },
                ListOfGens = generation._generations.Select(g => new
                {
                    Schedule = g._schedule,  // Zapisujemy postrzępioną tablicę
                    Fitness = g.Fitness,      // Publiczne właściwości
                    HoursScore = g.HoursScore
                    // ... tylko to, co nie jest ignorowane
                }).ToList(),
                Statistics = new
                {
                    generation.MaxFitness,
                    generation.Mean,
                    generation.Variation
                    // ... inne statystyki
                }
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JaggedArrayConverter() } // Konwerter dla int[][]
            };

            string json = JsonSerializer.Serialize(dataToSave, options);
            File.WriteAllText(savePath, json);


        }

        public void CreateEmptyManifest()
        {


            string manifestPath = Path.Combine(_manifestDirectory, "manifest.json");

          
            var manifest = new
            {
                LastCreatedFileId = 0,
                
            };

            
            string json = JsonSerializer.Serialize(manifest, new JsonSerializerOptions
            {
                WriteIndented = true
            });

           
            File.WriteAllText(manifestPath, json);



        }

        public void UpdateManifest()
        {


        }
     
        public int ReadManifest()
        {
            if(!File.Exists("manifest.json"))
            {
               CreateEmptyManifest();
            }


            string manifestPath = Path.Combine(_manifestDirectory, "manifest.json");

           

            try
            {
                // Odczytaj zawartość pliku
                string json = File.ReadAllText(manifestPath);

                // Deserializuj JSON do obiektu anonimowego
                var manifest = JsonSerializer.Deserialize<ManifestModel>(json);

                return manifest.LastCreatedFileId;
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
