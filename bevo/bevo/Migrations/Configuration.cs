namespace bevo.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using CsvHelper;
    using System.Reflection;
    using System.IO;
    using System.Text;
    using Models;

    internal sealed class Configuration : DbMigrationsConfiguration<bevo.DAL.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(bevo.DAL.AppDbContext context)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "bevo.SeedData.Person.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    CsvReader csvReader = new CsvReader(reader);
                    csvReader.Configuration.WillThrowOnMissingField = false;
                    var persons = csvReader.GetRecords<Person>().ToArray();
                    context.Persons.AddOrUpdate(c => c.Email, persons);
                }
            }

            resourceName = "bevo.SeedData.Payee.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    CsvReader csvReader = new CsvReader(reader);
                    csvReader.Configuration.WillThrowOnMissingField = false;
                    var payees = csvReader.GetRecords<Payee>().ToArray();
                    context.Payees.AddOrUpdate(p => p.Name, payees);
                }
            }

            resourceName = "bevo.SeedData.CheckingAccount.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    CsvReader csvReader = new CsvReader(reader);
                    csvReader.Configuration.WillThrowOnMissingField = false;
                    while(csvReader.Read())
                    {
                        var checkingAccount = csvReader.GetRecord<CheckingAccount>();
                        var personEmail = csvReader.GetField<string>("PersonEmail");
                        checkingAccount.Person = context.Persons.Local.Single(c => c.Email == personEmail);
                        context.CheckingAccounts.AddOrUpdate(p => p.Person, checkingAccount);
                    }
                }
            }

            resourceName = "bevo.SeedData.IRAccount.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    CsvReader csvReader = new CsvReader(reader);
                    csvReader.Configuration.WillThrowOnMissingField = false;
                    while (csvReader.Read())
                    {
                        var iRAccount = csvReader.GetRecord<IRAccount>();
                        var personEmail = csvReader.GetField<string>("PersonEmail");
                        iRAccount.Person = context.Persons.Local.Single(c => c.Email == personEmail);
                        context.IRAccounts.AddOrUpdate(p => p.Person, iRAccount);
                    }
                }
            }

            resourceName = "bevo.SeedData.SavingAccount.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    CsvReader csvReader = new CsvReader(reader);
                    csvReader.Configuration.WillThrowOnMissingField = false;
                    while (csvReader.Read())
                    {
                        var savingAccount = csvReader.GetRecord<SavingAccount>();
                        var personEmail = csvReader.GetField<string>("PersonEmail");
                        savingAccount.Person = context.Persons.Local.Single(c => c.Email == personEmail);
                        context.SavingAccounts.AddOrUpdate(p => p.Person, savingAccount);
                    }
                }
            }

            resourceName = "bevo.SeedData.StockPortfolio.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    CsvReader csvReader = new CsvReader(reader);
                    csvReader.Configuration.WillThrowOnMissingField = false;
                    while (csvReader.Read())
                    {
                        var stockPortfolio = csvReader.GetRecord<StockPortfolio>();
                        var personEmail = csvReader.GetField<string>("PersonEmail");
                        stockPortfolio.Person = context.Persons.Local.Single(c => c.Email == personEmail);
                        context.StockPortfolio.AddOrUpdate(p => p.Person, stockPortfolio);
                    }
                }
            }



        }
    }
}
