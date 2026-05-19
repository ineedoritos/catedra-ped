using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ProyectoCatedra.Utilidades
{
    public static class EscaladorDpi
    {
        public static void EscalarFormulario(Form form, float factor)
        {
            if (factor <= 1.0f) return;

            form.SuspendLayout();

            // 1. Guardar y desactivar anclajes
            var anclajes = new Dictionary<Control, AnchorStyles>();
            DesactivarAnclajes(form, anclajes);

            // 2. Escalar el formulario base
            form.Size = new Size(Escalar(form.Width, factor), Escalar(form.Height, factor));

            // 3. Escalar posiciones y tamaños
            EscalarControlesInternos(form, factor);

            // 4. Restaurar anclajes
            RestaurarAnclajes(anclajes);

            form.ResumeLayout();
        }

        private static void DesactivarAnclajes(Control padre, Dictionary<Control, AnchorStyles> anclajes)
        {
            foreach (Control control in padre.Controls)
            {
                anclajes[control] = control.Anchor;
                control.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                if (control.HasChildren) DesactivarAnclajes(control, anclajes);
            }
        }

        private static void RestaurarAnclajes(Dictionary<Control, AnchorStyles> anclajes)
        {
            foreach (var kvp in anclajes)
            {
                kvp.Key.Anchor = kvp.Value;
            }
        }

        // Mantenemos el método antiguo por compatibilidad si algo lo llama, 
        // pero delegamos su trabajo a la nueva logica de hijos.
        public static void EscalarJerarquia(Control raiz, float factor)
        {
            if (factor <= 1f) return;
            EscalarControlesInternos(raiz, factor);
        }

        private static void EscalarControlesInternos(Control padre, float factor)
        {
            foreach (Control control in padre.Controls)
            {
                if (control.Dock == DockStyle.None)
                {
                    control.Left = Escalar(control.Left, factor);
                    control.Top = Escalar(control.Top, factor);
                    control.Width = Escalar(control.Width, factor);
                    control.Height = Escalar(control.Height, factor);
                }
                else if (control.Dock == DockStyle.Top || control.Dock == DockStyle.Bottom)
                {
                    control.Height = Escalar(control.Height, factor);
                }
                else if (control.Dock == DockStyle.Left || control.Dock == DockStyle.Right)
                {
                    control.Width = Escalar(control.Width, factor);
                }

                control.Margin = new Padding(
                    Escalar(control.Margin.Left, factor),
                    Escalar(control.Margin.Top, factor),
                    Escalar(control.Margin.Right, factor),
                    Escalar(control.Margin.Bottom, factor));

                control.Padding = new Padding(
                    Escalar(control.Padding.Left, factor),
                    Escalar(control.Padding.Top, factor),
                    Escalar(control.Padding.Right, factor),
                    Escalar(control.Padding.Bottom, factor));

                if (control.Controls.Count > 0)
                {
                    EscalarControlesInternos(control, factor);
                }
            }
        }

        private static int Escalar(int valor, float factor)
        {
            return (int)Math.Round(valor * factor);
        }
    }
}
