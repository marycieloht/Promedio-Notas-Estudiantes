using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.IO;

namespace Gestión_de_Estudiantes_con_Promedio
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Configuración del DataGridView
            dgvEstudiantes.AutoGenerateColumns = false;
            dgvEstudiantes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEstudiantes.MultiSelect = false;
            dgvEstudiantes.AllowUserToAddRows = false;

            // Si no tiene columnas, crearlas (evita duplicados si el diseñador ya las tuviera)
            if (dgvEstudiantes.Columns.Count == 0)
            {
                dgvEstudiantes.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Nombre", HeaderText = "Nombre", Width = 180 });
                dgvEstudiantes.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Nota1", HeaderText = "Nota 1", Width = 80 });
                dgvEstudiantes.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Nota2", HeaderText = "Nota 2", Width = 80 });
                dgvEstudiantes.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Nota3", HeaderText = "Nota 3", Width = 80 });
                dgvEstudiantes.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Promedio", HeaderText = "Promedio", Width = 90 });
                dgvEstudiantes.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Estado", HeaderText = "Estado", Width = 120 });
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Ingrese el nombre del estudiante.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }

            if (!TryParseGrade(txtNota1.Text, out double n1))
            {
                MessageBox.Show("Nota 1 inválida. Debe ser un número entre 1.0 y 7.0.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNota1.Focus();
                return;
            }
            if (!TryParseGrade(txtNota2.Text, out double n2))
            {
                MessageBox.Show("Nota 2 inválida. Debe ser un número entre 1.0 y 7.0.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNota2.Focus();
                return;
            }
            if (!TryParseGrade(txtNota3.Text, out double n3))
            {
                MessageBox.Show("Nota 3 inválida. Debe ser un número entre 1.0 y 7.0.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNota3.Focus();
                return;
            }

            // cálculo del promedio
            double promedio = Math.Round((n1 + n2 + n3) / 3.0, 2); // redondeado a 2 decimales
            string estado = (promedio >= 4.0) ? "Aprobado" : "Reprobado";

            // Añadir fila al DataGridView
            dgvEstudiantes.Rows.Add(nombre, n1.ToString("F1"), n2.ToString("F1"), n3.ToString("F1"), promedio.ToString("F2"), estado);

            // Limpiar inputs
            txtNombre.Clear(); txtNota1.Clear(); txtNota2.Clear(); txtNota3.Clear();
            txtNombre.Focus();
        }

        private bool TryParseGrade(string text, out double grade)
        {
            grade = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;
            text = text.Trim().Replace(',', '.'); // aceptar coma o punto decimal
            if (!double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out grade)) return false;
            if (grade < 1.0 || grade > 7.0) return false;
            return true;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (dgvEstudiantes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione la fila que desea eliminar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            dgvEstudiantes.Rows.RemoveAt(dgvEstudiantes.SelectedRows[0].Index);
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (dgvEstudiantes.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Archivos de texto (.txt)|.txt", FileName = "estudiantes.txt" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(sfd.FileName))
                        {
                            sw.WriteLine("Nombre;Nota1;Nota2;Nota3;Promedio;Estado"); // cabecera
                            foreach (DataGridViewRow row in dgvEstudiantes.Rows)
                            {
                                if (row.IsNewRow) continue;
                                string nombre = row.Cells["Nombre"].Value?.ToString() ?? "";
                                string nota1 = row.Cells["Nota1"].Value?.ToString() ?? "";
                                string nota2 = row.Cells["Nota2"].Value?.ToString() ?? "";
                                string nota3 = row.Cells["Nota3"].Value?.ToString() ?? "";
                                string promedio = row.Cells["Promedio"].Value?.ToString() ?? "";
                                string estado = row.Cells["Estado"].Value?.ToString() ?? "";
                                sw.WriteLine($"{nombre};{nota1};{nota2};{nota3};{promedio};{estado}");
                            }
                        }
                        MessageBox.Show("Archivo exportado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al exportar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
