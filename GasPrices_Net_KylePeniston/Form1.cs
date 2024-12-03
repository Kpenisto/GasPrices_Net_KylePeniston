using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GasPrices_Net_KylePeniston
{
    public partial class Form1 : Form
    {
        List<Gas> gasList = new List<Gas>();

        string[] monthNames = {
            "", "January", "February", "March", "April", "May",
            "June", "July", "August", "September", "October",
            "November", "December"
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StreamReader infile;

            DateTime date;
            decimal price;

            try
            {
                infile = File.OpenText("GasPrices.txt");

                while (!infile.EndOfStream) { 
                    string line = infile.ReadLine();

                    var tokens = line.Split(':');

                    if (tokens.Length == 2)
                    {

                        if (DateTime.TryParse(tokens[0], out date))
                        {
                            if (decimal.TryParse(tokens[1], out price))
                            {
                                Gas gas = new Gas()
                                {
                                    Date = date,
                                    Price = price
                                };

                                gasList.Add(gas);
                            }
                            else
                            {
                                MessageBox.Show($"Bad price data encountered in the file:\n{line}");
                            }
                        }
                        else
                        {
                            MessageBox.Show($"A bad date encountered in the file:\n{line}");
                        }
                    }
                    else {
                        MessageBox.Show($"Bad data encountered in the file:\n{line}");
                    }
                }
                infile.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        //Avg price per year
        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            var years = from g in gasList
                        orderby g.Date.Year
                        select g.Date.Year;

            var uniqueYears = years.Distinct();

            foreach (int year in uniqueYears)
            {
                var gasPrices = from g in gasList
                                where g.Date.Year == year
                                select g.Price;

                decimal avg = gasPrices.Average();

                listBox1.Items.Add($"{year}: {avg:c}");
            }
        }

        //Avg price per month
        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            var months = from g in gasList
                        orderby g.Date.Month
                        select g.Date.Month;

            var uniqueMonths = months.Distinct();

            foreach (int month in uniqueMonths)
            {
                var gasPrices = from g in gasList
                                where g.Date.Month == month
                                select g.Price;

                decimal avg = gasPrices.Average();

                listBox1.Items.Add($"{monthNames[month]}: {avg:c}");
            }
        }

        //Highest & Lowest Per Year
        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            var years = from g in gasList
                        orderby g.Date.Year
                        select g.Date.Year;

            var uniqueYears = years.Distinct();

            foreach (int year in uniqueYears)
            {
                var gasPrices = from g in gasList
                                where g.Date.Year == year
                                select g.Price;

                decimal highest = gasPrices.Max();
                decimal lowest = gasPrices.Min();

                listBox1.Items.Add($"{year} highest: {highest:c}");
                listBox1.Items.Add($"{year} lowest: {lowest:c}");
            }
        }

        //Prices Lowest to Highest
        private void button4_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            var results = from gas in gasList
                          orderby gas.Price
                          select gas;

            foreach (Gas g in results)
            {
                listBox1.Items.Add($"{g.Date.ToShortDateString()}; {g.Price:c}");
            }

            try
            {
                var outfile = File.CreateText("prices_lowest_to_highest.txt");

                foreach (Gas g in results) { 
                    outfile.WriteLine($"{g.Date.ToShortDateString()}: {g.Price:c}");
                }

                outfile.Close();
                MessageBox.Show("Results also written to prices_lowest_to_highest.txt");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        //Prices Highest to Lowest
        private void button6_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            var results = from gas in gasList
                          orderby gas.Price descending
                          select gas;

            foreach (Gas g in results)
            {
                listBox1.Items.Add($"{g.Date.ToShortTimeString()}; {g.Price:c}");
            }

            try
            {
                var outfile = File.CreateText("prices_highest_to_lowest.txt");

                foreach (Gas g in results)
                {
                    outfile.WriteLine($"{g.Date.ToShortTimeString()}: {g.Price:c}");
                }

                outfile.Close();
                MessageBox.Show("Results also written to prices_highest_to_lowest.txt");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        //Close form
        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Price Range
        private void button7_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            decimal minPrice, maxPrice;

            if (!decimal.TryParse(textBox1.Text, out minPrice))
            {
                MessageBox.Show("Invalid minimum price. Please enter a valid number.");
                return;
            }

            if (!decimal.TryParse(textBox2.Text, out maxPrice))
            {
                MessageBox.Show("Invalid maximum price. Please enter a valid number.");
                return;
            }

            if (minPrice > maxPrice)
            {
                MessageBox.Show("Minimum price cannot be greater than maximum price.");
                return;
            }

            var filteredResults = from gas in gasList
                                  where gas.Price >= minPrice && gas.Price <= maxPrice
                                  orderby gas.Price
                                  select gas;

            foreach (Gas g in filteredResults)
            {
                listBox1.Items.Add($"{g.Date.ToShortDateString()}: {g.Price:c}");
            }

            if (!filteredResults.Any())
            {
                listBox1.Items.Add("No results found in the specified price range.");
            }
        }
    }
}
