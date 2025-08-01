using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;

namespace ListaTareas.MVVM.Models
{
    public class TareasRepository
    {
        private readonly FirebaseClient _client;

        public TareasRepository()
        {
            _client = new FirebaseClient("https://fir-maui-67420-default-rtdb.firebaseio.com/");
        }

        //1. Crear el documento
        public async Task CreateDocumentAsync(TareaModel tarea)
        {
            await _client.Child("Tareas").PostAsync(tarea);

            Console.WriteLine($"La Tarea {tarea.Nombre} creada exitosamente!");
        }

        //2. Método GetAll
        public async Task<Dictionary<string, TareaModel>> GetAllAsync()
        {
            try
            {
                var lstTareas = await _client.Child("Tareas").OnceAsync<TareaModel>();
                var TareaDictionary = new Dictionary<string, TareaModel>();

                foreach (var Tarea in lstTareas)
                {
                    TareaDictionary.Add(Tarea.Key, Tarea.Object);
                }

                return TareaDictionary;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar los datos: {ex.Message}");
                return new Dictionary<string, TareaModel>();
            }
        }

        // 3. Método Update
        public async Task UpdateDocumentAsync(TareaModel Tarea, string Key)
        {
            await _client.Child("Tareas").Child(Key).PatchAsync(Tarea);
            Console.WriteLine($"La Tarea {Tarea.Nombre} se actualizo exitosamente");
        }

        // 4. Método Delete
        public async Task DeleteDocumentAsync(string Key)
        {
            await _client.Child("Tareas").Child(Key).DeleteAsync();
            Console.WriteLine("La Tarea ha sido eliminada");
        }
    }
}
