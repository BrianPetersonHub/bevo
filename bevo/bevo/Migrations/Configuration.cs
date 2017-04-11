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

    internal sealed class Configuration : DbMigrationsConfiguration<bevo.Models.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }


        //TODO: Edit seed method to work with identity
        /*
        protected override void Seed(bevo.Models.AppDbContext context)
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
                        checkingAccount.AppUser = context.Persons.Local.Single(c => c.Email == personEmail);
                        context.CheckingAccounts.AddOrUpdate(p => p.AppUser, checkingAccount);
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
                        iRAccount.AppUser = context.Persons.Local.Single(c => c.Email == personEmail);
                        context.IRAccounts.AddOrUpdate(p => p.AppUser, iRAccount);
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
                        savingAccount.AppUser = context.Users.Local.Single(c => c.Email == personEmail);
                        context.SavingAccounts.AddOrUpdate(p => p.AppUser, savingAccount);
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
                        stockPortfolio.AppUser = context.Users.Local.Single(c => c.Email == personEmail);
                        context.StockPortfolios.AddOrUpdate(p => p.AppUser, stockPortfolio);
                    }
                }
            }


        }
        */
    }
}
