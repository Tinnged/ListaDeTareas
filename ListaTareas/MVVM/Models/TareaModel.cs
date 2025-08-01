using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListaTareas.MVVM.Models
{
    public class TareaModel
    {
        public string Key { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public bool Estado { get; set; }

        public string EstadoTexto => Estado ? "Estado: Completada" : "Estado: Pendiente";
        public Color EstadoColor => Estado ? Colors.Green : Colors.Yellow;
        public string BotonTexto => Estado ? "Pendiente" : "Completar";
        public Color BotonColor => Estado ? Colors.Yellow : Colors.Green;

        public bool EsVencida => !Estado && FechaVencimiento.Date < DateTime.Today;
        public Color ColorFecha => EsVencida ? Colors.Red : Colors.Gray;
        public string TextoFecha => EsVencida ? $"⚠️ Vencida: {FechaVencimiento:dd/MM/yyyy}" : $"Vence: {FechaVencimiento:dd/MM/yyyy}";
    }
}
