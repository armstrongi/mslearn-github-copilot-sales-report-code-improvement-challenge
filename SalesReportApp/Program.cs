using System.Diagnostics.Contracts;
using System.Globalization;

namespace ReportGenerator
{
    class QuarterlyIncomeReport
    {
        static void Main(string[] args)
        {
            // Set the culture to en-US
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            if (!IsUserAuthenticated())
            {
                Console.WriteLine("Access denied. User is not authenticated.");
                return;
            }

            // create a new instance of the class
            QuarterlyIncomeReport report = new QuarterlyIncomeReport();

            // call the GenerateSalesData method
            SalesData[] salesData = report.GenerateSalesData();

            // call the QuarterlySalesReport method
            report.QuarterlySalesReport(salesData);
        }

        private static bool IsUserAuthenticated()
        {
            // Implement your authentication logic here
            // For demonstration purposes, let's assume the user is authenticated
            return true;
        }

        /* public struct SalesData includes the following fields: date sold, department name, product ID, quantity sold, unit price */
        public struct SalesData
        {
            public DateOnly dateSold;
            public string departmentName;
            public string productID;
            public int quantitySold;
            public double unitPrice;
            public double baseCost;
            public int volumeDiscount;
        }        

        public struct QuarterlySalesDataByDepartment
        {
            public string Quarter;
            public string Department;
            public double Sales;
            public double Profit;
            public double ProfitPercentage;
        }
        
        public struct ProdDepartments
        {
            public static string[] departmentNames = ["Men's Clothing", "Women's Clothing", "Children's Clothing", "Accessories", "Footwear", "Outerwear", "Sportswear", "Undergarments"];
            public static string[] departmentAbbreviations = ["MENS", "WOMN", "CHLD", "ACCS", "FOOT", "OUTR", "SPRT", "UNDR"];
        }

        public struct ManufacturingSites
        {
            public static string[] manufacturingSites = ["US1", "US2", "US3", "UK1", "UK2", "UK3", "JP1", "JP2", "JP3", "CA1"];
        }

        /* the GenerateSalesData method returns 1000 SalesData records. It assigns random values to each field of the data structure */
        public SalesData[] GenerateSalesData()
        {
            SalesData[] salesData = new SalesData[100000];
            Random random = new Random();

            for (int i = 0; i < 100000; i++)
            {
                salesData[i].dateSold = new DateOnly(2023, random.Next(1, 13), random.Next(1, 29));
                salesData[i].departmentName = ProdDepartments.departmentNames[random.Next(0, ProdDepartments.departmentNames.Length)];
                salesData[i].productID = ConstructProductId(salesData[i], random);
                salesData[i].quantitySold = random.Next(1, 101);
                salesData[i].unitPrice = random.Next(25, 300) + random.NextDouble();
                salesData[i].baseCost = salesData[i].unitPrice * (1 - (random.Next(5, 21) / 100.0));
                salesData[i].volumeDiscount = (int)(salesData[i].quantitySold * 0.1);

            }

            return salesData;
        }

        public static string ConstructProductId(SalesData salesDataItem, Random random)
        {
            int indexOfDept = Array.IndexOf(ProdDepartments.departmentNames, salesDataItem.departmentName);
            string deptAbb = ProdDepartments.departmentAbbreviations[indexOfDept];
            string firstDigit = (indexOfDept + 1).ToString();
            string nextTwoDigits = random.Next(1, 100).ToString("D2");
            string sizeCode = new string[] { "XS", "S", "M", "L", "XL" }[random.Next(0, 5)];
            string colorCode = new string[] { "BK", "BL", "GR", "RD", "YL", "OR", "WT", "GY" }[random.Next(0, 8)];
            string manufacturingSite = ManufacturingSites.manufacturingSites[random.Next(0, ManufacturingSites.manufacturingSites.Length)];
        
            return $"{deptAbb}-{firstDigit}{nextTwoDigits}-{sizeCode}-{colorCode}-{manufacturingSite}";
        }
        public static string[,] DeconstructProductId(string productID)
        {
            string[] parts = productID.Split('-');
            string[,] deconstructedProduct = new string[5, 2];

            deconstructedProduct[0, 0] = "Department Abbreviation";
            deconstructedProduct[0, 1] = parts[0];

            deconstructedProduct[1, 0] = "Product Serial Number";
            deconstructedProduct[1, 1] = parts[1];

            deconstructedProduct[2, 0] = "Size Code";
            deconstructedProduct[2, 1] = parts[2];

            deconstructedProduct[3, 0] = "Color Code";
            deconstructedProduct[3, 1] = parts[3];

            deconstructedProduct[4, 0] = "Manufacturing Site";
            deconstructedProduct[4, 1] = parts[4];

            return deconstructedProduct;
        }

        public void QuarterlySalesReport(SalesData[] salesData)
        {                        
            // create an array to hold the quarterly sales data by department            
            QuarterlySalesDataByDepartment[] quarterlySalesDataByDepartment = new QuarterlySalesDataByDepartment[ProdDepartments.departmentNames.Length * 4];

            // create a dictionary to store the quarterly top profit for product numbers
            var quarterlyTopProfitForProductNumbers = new Dictionary<string, Dictionary<string, (int, double, double, double, double)>>();            
            
            // summarise the sales data
            SummariseSalesData(salesData, quarterlySalesDataByDepartment, quarterlyTopProfitForProductNumbers);

            // display the quarterly sales report
            DisplaySalesReports(salesData, quarterlySalesDataByDepartment, quarterlyTopProfitForProductNumbers);
        }

        public static string GetQuarter(int month)
        {
            if (month >= 1 && month <= 3)
            {
                return "Q1";
            }
            else if (month >= 4 && month <= 6)
            {
                return "Q2";
            }
            else if (month >= 7 && month <= 9)
            {
                return "Q3";
            }
            else
            {
                return "Q4";
            }
        }

        public static int GetQuarterIndex(string quarter)
        {
            switch (quarter)
            {
                case "Q1":
                    return 0;
                case "Q2":
                    return 1;
                case "Q3":
                    return 2;
                default:
                    return 3;
            }
        }        

        public static void InitializeQuarterlySalesDataByDepartment(QuarterlySalesDataByDepartment[] quarterlySalesDataByDepartment)
        {
            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < ProdDepartments.departmentNames.Length; j++)
                {
                    quarterlySalesDataByDepartment[index].Quarter = "Q" + (i + 1).ToString();
                    quarterlySalesDataByDepartment[index].Department = ProdDepartments.departmentNames[j];
                    quarterlySalesDataByDepartment[index].Sales = 0;
                    quarterlySalesDataByDepartment[index].Profit = 0;
                    quarterlySalesDataByDepartment[index].ProfitPercentage = 0;
                    index++;
                }
            }
        }        

        public static void AccumulateQuarterlySalesDataByDepartment(QuarterlySalesDataByDepartment[] quarterlySalesDataByDepartment, string quarter, string department, double totalSales, double profit)
        {
            try
            {
                if (quarterlySalesDataByDepartment == null)
                {
                    throw new ArgumentNullException(nameof(quarterlySalesDataByDepartment), "Quarterly sales data by department cannot be null.");
                }

                if (string.IsNullOrEmpty(quarter))
                {
                    throw new ArgumentException("Quarter cannot be null or empty.", nameof(quarter));
                }

                if (string.IsNullOrEmpty(department))
                {
                    throw new ArgumentException("Department cannot be null or empty.", nameof(department));
                }

                if (totalSales < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(totalSales), "Total sales cannot be negative.");
                }                

                int quarterIndex = GetQuarterIndex(quarter);
                int departmentIndex = Array.IndexOf(ProdDepartments.departmentNames, department);

                if (departmentIndex == -1)
                {
                    throw new ArgumentException("Invalid department name.", nameof(department));
                }

                int index = quarterIndex * ProdDepartments.departmentNames.Length + departmentIndex;
            
                quarterlySalesDataByDepartment[index].Sales += totalSales;
                quarterlySalesDataByDepartment[index].Profit += profit;
                quarterlySalesDataByDepartment[index].ProfitPercentage = quarterlySalesDataByDepartment[index].Profit / quarterlySalesDataByDepartment[index].Sales * 100;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accumulating quarterly sales data by department: {ex.Message}");
            }
        }        

        public static void ReportQuarterSalesDataByDepartment(QuarterlySalesDataByDepartment[] quarterlySalesDataByDepartment, string quarter)
        {
            // display the quarterly sales, profit, and profit percentage by department            
            Console.WriteLine(quarter + " Sales by Department:");  

            var departmentsForQuarter = quarterlySalesDataByDepartment
                .Where(q => q.Quarter == quarter)
                .OrderBy(d => d.Department);

            // Print table headers
            Console.WriteLine("┌───────────────────────┬───────────────────┬───────────────────┬───────────────────┐");
            Console.WriteLine("│      Department       │       Sales       │       Profit      │ Profit Percentage │");
            Console.WriteLine("├───────────────────────┼───────────────────┼───────────────────┼───────────────────┤");

            double sales = 0;
            double profit = 0;

            foreach (var department in departmentsForQuarter)
            {
                sales += department.Sales;
                profit += department.Profit;

                string formattedDepartmentSalesAmount = department.Sales.ToString("C");
                string formattedDepartmentProfitAmount = department.Profit.ToString("C");
                string formattedDepartmentProfitPercentage = department.ProfitPercentage.ToString("F2");

                Console.WriteLine("│ {0,-22}│ {1,17} │ {2,17} │ {3,16}% │", department.Department, formattedDepartmentSalesAmount, formattedDepartmentProfitAmount, formattedDepartmentProfitPercentage);
            }            

            double profitPercentage = profit / sales * 100;
            
            string formattedSalesAmount = sales.ToString("C");        
            string formattedProfitAmount = profit.ToString("C");
            string formattedProfitPercentage = profitPercentage.ToString("F2");

            Console.WriteLine("├───────────────────────┼───────────────────┼───────────────────┼───────────────────┤");
            Console.WriteLine("│ {0,-22}│ {1,17} │ {2,17} │ {3,16}% │", "Total", formattedSalesAmount, formattedProfitAmount, formattedProfitPercentage);

            Console.WriteLine("└───────────────────────┴───────────────────┴───────────────────┴───────────────────┘");
            Console.WriteLine();            
        }

        public static void ReportTop3SalesData(SalesData[] salesData, string quarter)
        {
            var top3SalesProducts = salesData
                .Where(data => GetQuarter(data.dateSold.Month) == quarter)
                .OrderByDescending(data => (data.quantitySold * data.unitPrice) - (data.quantitySold * data.baseCost))
                .Take(3)
                .ToList();

            // display the top 3 sales orders for the quarter
            Console.WriteLine(quarter + " Top 3 Sales Orders:");            

            // Print table headers
            Console.WriteLine("┌───────────────────────┬───────────────────┬───────────────────┬───────────────────┬───────────────────┬───────────────────┐");
            Console.WriteLine("│      Product ID       │   Quantity Sold   │    Unit Price     │   Total Sales     │      Profit       │ Profit Percentage │");
            Console.WriteLine("├───────────────────────┼───────────────────┼───────────────────┼───────────────────┼───────────────────┼───────────────────┤");

            foreach (SalesData salesOrder in top3SalesProducts)
            {
                double orderTotalSales = salesOrder.quantitySold * salesOrder.unitPrice;
                double orderProfit = orderTotalSales - (salesOrder.quantitySold * salesOrder.baseCost);
                double orderProfitPercentage = (orderProfit / orderTotalSales) * 100;

                Console.WriteLine("│ {0,-22}│ {1,17} │ {2,17} │ {3,17} │ {4,17} │ {5,16}% │", salesOrder.productID, salesOrder.quantitySold, salesOrder.unitPrice.ToString("C"), orderTotalSales.ToString("C"), orderProfit.ToString("C"), orderProfitPercentage.ToString("F2"));
            }

            Console.WriteLine("└───────────────────────┴───────────────────┴───────────────────┴───────────────────┴───────────────────┴───────────────────┘");
            Console.WriteLine();
        }

        public static void AccumulateTopProfitProductNumbers(Dictionary<string, Dictionary<string, (int, double, double, double, double)>> quarterlyTopProfitForProductNumbers, SalesData data, string quarter)
        {
            try
            {
                if (quarterlyTopProfitForProductNumbers == null)
                {
                    throw new ArgumentNullException(nameof(quarterlyTopProfitForProductNumbers), "Quarterly top profit for product numbers cannot be null.");
                }

                if (string.IsNullOrEmpty(data.productID))
                {
                    throw new ArgumentNullException(nameof(data), "Product ID cannot be null or empty.");
                }

                if (data.quantitySold < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(data.quantitySold), "Quantity sold must not be negative.");
                }

                if (data.unitPrice <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(data.unitPrice), "Unit price must be greater than zero.");
                }

                if (data.baseCost <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(data.baseCost), "Base cost must be greater than zero.");
                }                

                if (string.IsNullOrEmpty(quarter))
                {
                    throw new ArgumentException("Quarter cannot be null or empty.", nameof(quarter));
                }

                string[,] deconstructedProductId = DeconstructProductId(data.productID);
                string productSerialNumber = deconstructedProductId[0, 1] + "-" + deconstructedProductId[1, 1] + "-ss-cc-mmm";
            
                double totalSales = data.quantitySold * data.unitPrice;
                double profit = totalSales - (data.quantitySold * data.baseCost);
                double profitPercentage = (profit / totalSales) * 100;
            
                if (!quarterlyTopProfitForProductNumbers.ContainsKey(quarter))
                {
                    quarterlyTopProfitForProductNumbers.Add(quarter, new Dictionary<string, (int, double, double, double, double)>());
                }
            
                if (quarterlyTopProfitForProductNumbers[quarter].ContainsKey(productSerialNumber))
                {
                    var (unitsSold, totalSalesAmount, unitCost, totalProfit, innerProfitPercentage) = quarterlyTopProfitForProductNumbers[quarter][productSerialNumber];
                    unitsSold += data.quantitySold;
                    totalSalesAmount += totalSales;
                    unitCost = totalSalesAmount / unitsSold;
                    totalProfit += profit;
                    innerProfitPercentage = (totalProfit / totalSalesAmount) * 100;
                    quarterlyTopProfitForProductNumbers[quarter][productSerialNumber] = (unitsSold, totalSalesAmount, unitCost, totalProfit, innerProfitPercentage);
                }
                else
                {
                    quarterlyTopProfitForProductNumbers[quarter].Add(productSerialNumber, (data.quantitySold, totalSales, data.baseCost, profit, profitPercentage));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accumulating top profit product numbers: {ex.Message}");
            }
        }

        public static void ReportTopProfitProductNumbers(Dictionary<string, (int, double, double, double, double)> quarterlyTopProfitForProductNumbers, string quarter)
        {
            // display the quarterly top profit for product numbers
            Console.WriteLine(quarter + " Top Profit for Product Numbers:");
            var quarterlyTopProfit = quarterlyTopProfitForProductNumbers;

            // Sort the quarterly top profit by profit in descending order
            var sortedQuarterlyTopProfit = quarterlyTopProfit.OrderByDescending(p => p.Value.Item4).Take(3);

            // Print table headers
            Console.WriteLine("┌───────────────────────┬───────────────────┬───────────────────┬───────────────────┬───────────────────┬───────────────────┐");
            Console.WriteLine("│   Product Serial No   │    Units Sold     │   Total Sales     │    Unit Cost      │     Total Profit  │ Profit Percentage │");
            Console.WriteLine("├───────────────────────┼───────────────────┼───────────────────┼───────────────────┼───────────────────┼───────────────────┤");

            foreach (KeyValuePair<string, (int, double, double, double, double)> product in sortedQuarterlyTopProfit)
            {
                var (unitsSold, totalSalesAmount, unitCost, totalProfit, profitPercentage) = product.Value;

                Console.WriteLine("│ {0,-22}│ {1,17} │ {2,17} │ {3,17} │ {4,17} │ {5,16}% │", product.Key, unitsSold, totalSalesAmount.ToString("C"), unitCost.ToString("C"), totalProfit.ToString("C"), profitPercentage.ToString("F2"));
            }

            Console.WriteLine("└───────────────────────┴───────────────────┴───────────────────┴───────────────────┴───────────────────┴───────────────────┘");
            Console.WriteLine();
        }

        public void DisplaySalesReports(SalesData[] salesData, QuarterlySalesDataByDepartment[] quarterlySalesDataByDepartment, Dictionary<string, Dictionary<string, (int, double, double, double, double)>> quarterlyTopProfitForProductNumbers)
        {
            // create and populate a variable to hold each quarter in salesData
            var salesDataByQuarter = salesData
                .GroupBy(data => GetQuarter(data.dateSold.Month))
                .OrderBy(group => group.Key)
                .ToList();            

            // iterate through the quarterly sales data
            foreach (var quarterGroup in salesDataByQuarter)
            {
                string quarter = quarterGroup.Key;
                
                // display the quarterly sales, profit, and profit percentage by department                
                ReportQuarterSalesDataByDepartment(quarterlySalesDataByDepartment, quarter);

                // display the top 3 sales orders for the quarter                
                ReportTop3SalesData(salesData, quarter);

                // display the quarterly top profit for product numbers
                ReportTopProfitProductNumbers(quarterlyTopProfitForProductNumbers[quarter], quarter);
            }
        }

        public void SummariseSalesData(SalesData[] salesData, QuarterlySalesDataByDepartment[] quarterlySalesDataByDepartment, Dictionary<string, Dictionary<string, (int, double, double, double, double)>> quarterlyTopProfitForProductNumbers)
        {
            // Initialize the quarterly sales data for each quarter by department            
            InitializeQuarterlySalesDataByDepartment(quarterlySalesDataByDepartment);                        

            // iterate through the sales data
            foreach (SalesData data in salesData)
            {
                // calculate the total sales for each quarter
                string quarter = GetQuarter(data.dateSold.Month);
                double totalSales = data.quantitySold * data.unitPrice;
                double totalCost = data.quantitySold * data.baseCost;
                double profit = totalSales - totalCost;
                double profitPercentage = (profit / totalSales) * 100;                
                
                // accumulate the quarterly sales data by department
                AccumulateQuarterlySalesDataByDepartment(quarterlySalesDataByDepartment, quarter, data.departmentName, totalSales, profit);                                
                
                // accumulate the quarterly top profit for product numbers
                AccumulateTopProfitProductNumbers(quarterlyTopProfitForProductNumbers, data, quarter);
            }
        }
    }
}
