using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

// სხვადასხვა კატეგორიებისთვის Enum-ების განსაზღვრა
public enum Gender { Male, Female }
public enum Country { Georgia, USA, Germany, France } 
public enum City { Tbilisi, Batumi, Kutaisi } 
public enum GuarantorRelation { Mother, Father, Spouse, Friend, Relative, Other }
public enum Currency { GEL, USD, EUR }

// მთავარი კლასი პროგრამის გასაშვებად
class HelloWorld {
    static void Main()
    {
        // ინდივიდუალური მომხმარებლების შექმნა ობიექტის ინიციალიზატორებით მარტივობისთვის
        var person1 = new Person
        {
            FirstName = "Giorgi",
            LastName = "Abashidze",
            Gender = Gender.Male,
            PersonalNumber = "01010112345",
            DateOfBirth = new DateTime(1985, 5, 20),
            Country = Country.Georgia,
            City = City.Tbilisi,
            PhoneNumber = "599123456",
            Email = "giorgi.abashidze@example.com"
        };

        var person2 = new Person
        {
            FirstName = "Nino",
            LastName = "Kvaratskhelia",
            Gender = Gender.Female,
            PersonalNumber = "02020223456",
            DateOfBirth = new DateTime(1990, 3, 15),
            Country = Country.Georgia,
            City = City.Batumi,
            PhoneNumber = "555987654",
            Email = "nino.kvaratskhelia@example.com"
        };

        var individualClient1 = new IndividualClient { PersonDetails = person1 };
        var individualClient2 = new IndividualClient { PersonDetails = person2 };

        // კორპორატიული მომხმარებლების შექმნა
        var company1 = new Company
        {
            Name = "TBC",
            RegistrationCode = "123456789",
            RegistrationDate = new DateTime(2000, 1, 1),
            RegistrationCountry = Country.Georgia,
            RegistrationCity = City.Tbilisi,
            ContactPhoneNumber = "0322272727",
            ContactEmail = "contact@tbcbank.ge",
            ContactPerson = "Irakli Bairamashvili"
        };

        var company2 = new Company
        {
            Name = "BOG",
            RegistrationCode = "987654321",
            RegistrationDate = new DateTime(1995, 6, 15),
            RegistrationCountry = Country.Georgia,
            RegistrationCity = City.Tbilisi,
            ContactPhoneNumber = "0322444444",
            ContactEmail = "info@bog.ge",
            ContactPerson = "Levan Kipiani"
        };

        var corporateClient1 = new CorporateClient { CompanyDetails = company1 };
        var corporateClient2 = new CorporateClient { CompanyDetails = company2 };

        // მომხმარებლების დირექტორიაში დამატება
        ClientDirectory.AddClient(individualClient1);
        ClientDirectory.AddClient(individualClient2);
        ClientDirectory.AddClient(corporateClient1);
        ClientDirectory.AddClient(corporateClient2);

        // დეპოზიტებისა და სესხების დამატება
        var deposit1 = new Deposit
        {
            Client = individualClient1,
            DepositDate = DateTime.Now,
            Amount = 5000m,
            Currency = Currency.GEL,
            InterestRate = 5m,
            TermInMonths = 12
        };

        var deposit2 = new Deposit
        {
            Client = individualClient2,
            DepositDate = DateTime.Now,
            Amount = 10000m,
            Currency = Currency.USD,
            InterestRate = 6m,
            TermInMonths = 24
        };

        var loan1 = new Loan
        {
            Client = corporateClient1,
            LoanDate = DateTime.Now,
            Amount = 100000m,
            Currency = Currency.GEL,
            InterestRate = 7m,
            RemainingAmount = 80000m,
            TermInMonths = 36
        };

        var loan2 = new Loan
        {
            Client = corporateClient2,
            LoanDate = DateTime.Now,
            Amount = 200000m,
            Currency = Currency.USD,
            InterestRate = 8m,
            RemainingAmount = 180000m,
            TermInMonths = 48
        };

        individualClient1.AddDeposit(deposit1);
        individualClient2.AddDeposit(deposit2);
        corporateClient1.AddLoan(loan1);
        corporateClient2.AddLoan(loan2);

        // დეპოზიტებისა და სესხების დახარისხება Comparers და ენკაფსულაციის გამოყენებით
        BankingOperations.SortDeposits(individualClient1.Deposits);
        BankingOperations.SortDeposits(individualClient2.Deposits);
        BankingOperations.SortLoans(corporateClient1.Loans);
        BankingOperations.SortLoans(corporateClient2.Loans);

        // ყველაზე მაღალი დავალიანების მქონე კორპორატიული მომხმარებლების მიღება ენკაფსულაციისა და LINQ გამოყენებით
        var topDebtors = BankingOperations.GetTopDebtCorporateClients(2);
        Console.WriteLine("Top Corporate Debtors:");
        foreach (var debtor in topDebtors)
        {
            Console.WriteLine($"Company: {debtor.CompanyDetails.Name}, Remaining Debt: {debtor.Loans.Sum(l => l.RemainingAmount)}");
        }

        // წლიური სესხების ანგარიშის მიღება ენკაფსულაციისა და LINQ გამოყენებით
        decimal totalIndividualLoans, totalCorporateLoans;
        var totalLoans = BankingOperations.GetAnnualLoanReport(2024, out totalIndividualLoans, out totalCorporateLoans);

        Console.WriteLine($"\nTotal Loans in 2024: {totalLoans}");
        Console.WriteLine($"Total Individual Loans in 2024: {totalIndividualLoans}");
        Console.WriteLine($"Total Corporate Loans in 2024: {totalCorporateLoans}");
    }
}

// ინტერფეისი მომხმარებელთან დაკავშირებული ოპერაციებისთვის პოლიმორფიზმის დემონსტრაციისთვის
public interface IClient
{
    void AddClient(Client client);
    void EditClient(Client client);
    void DeleteClient(Guid clientId);
    void AddGuarantor(Guid clientId, Guid guarantorId);
    void RemoveGuarantor(Guid clientId, Guid guarantorId);
    Client GetClientInfo(Guid clientId);
}

// Person კლასი ინდივიდუალური ინფორმაციის შესანახად, ენკაფსულაციისა და მონაცემთა ვალიდაციის დემონსტრაციისთვის
public class Person
{
    [Key]
    public Guid Id { get; set; }

    [Required, MinLength(2), MaxLength(50)]
    public string FirstName { get; set; }

    [Required, MinLength(2), MaxLength(50)]
    public string LastName { get; set; }

    [Required]
    public Gender Gender { get; set; }

    [Required, StringLength(11, MinimumLength = 11)]
    public string PersonalNumber { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public Country Country { get; set; }

    [Required]
    public City City { get; set; }

    [Required, MinLength(4), MaxLength(50)]
    public string PhoneNumber { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    public GuarantorRelation? GuarantorRelation { get; set; }
    public Guid? GuarantorId { get; set; }

    public Person()
    {
        Id = Guid.NewGuid();
    }

    // ასაკის ვალიდაციის მეთოდი ენკაფსულაციის დემონსტრაციისთვის
    public bool IsValid()
    {
        var age = DateTime.Today.Year - DateOfBirth.Year;
        return age >= 18 && age <= 65;
    }
}

// Company კლასი კორპორატიული ინფორმაციის შესანახად, ენკაფსულაციისა და მონაცემთა ვალიდაციის დემონსტრაციისთვის
public class Company
{
    [Key]
    public Guid Id { get; set; }

    [Required, MinLength(2), MaxLength(50)]
    public string Name { get; set; }

    [Required, StringLength(9, MinimumLength = 9)]
    public string RegistrationCode { get; set; }

    [Required]
    public DateTime RegistrationDate { get; set; }

    [Required]
    public Country RegistrationCountry { get; set; }

    [Required]
    public City RegistrationCity { get; set; }

    [Required, MinLength(4), MaxLength(50)]
    public string ContactPhoneNumber { get; set; }

    [Required, EmailAddress]
    public string ContactEmail { get; set; }

    [Required, MinLength(4), MaxLength(50)]
    public string ContactPerson { get; set; }

    public Company()
    {
        Id = Guid.NewGuid();
    }
}

// დეპოზიტის კლასის განსაზღვრა, ენკაფსულაციის დემონსტრაციისთვის
public class Deposit
{
    public Client Client { get; set; }
    public DateTime DepositDate { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public decimal InterestRate { get; set; }
    public int TermInMonths { get; set; }
}

// სესხის კლასის განსაზღვრა, ენკაფსულაციის დემონსტრაციისთვის
public class Loan
{
    public Client Client { get; set; }
    public DateTime LoanDate { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public decimal InterestRate { get; set; }
    public decimal RemainingAmount { get; set; }
    public int TermInMonths { get; set; }
}

// აბსტრაქტული ძირითადი კლასი მომხმარებლებისთვის, მემკვიდრეობის დემონსტრაციისთვის
public abstract class Client
{
    public Guid Id { get; set; }
    public List<Deposit> Deposits { get; private set; }
    public List<Loan> Loans { get; private set; }

    public Client()
    {
        Id = Guid.NewGuid();
        Deposits = new List<Deposit>();
        Loans = new List<Loan>();
    }

    // დეპოზიტების და სესხების დამატების მეთოდები, ენკაფსულაციის დემონსტრაციისთვის
    public void AddDeposit(Deposit deposit)
    {
        Deposits.Add(deposit);
    }

    public void AddLoan(Loan loan)
    {
        Loans.Add(loan);
    }
}

// Derived კლასი ინდივიდუალური მომხმარებლებისთვის, მემკვიდრეობის დემონსტრაციისთვის
public class IndividualClient : Client
{
    public Person PersonDetails { get; set; }
}

// Derived კლასი კორპორატიული მომხმარებლებისთვის, მემკვიდრეობის დემონსტრაციისთვის
public class CorporateClient : Client
{
    public Company CompanyDetails { get; set; }
}

// სტატიკური კლასი მომხმარებელთა დირექტორიის სამართავად, ენკაფსულაციის დემონსტრაციისთვის
public static class ClientDirectory
{
    private static List<Client> clients = new List<Client>();

    // მომხმარებლის დირექტორიაში დამატების მეთოდი, ენკაფსულაციის დემონსტრაციისთვის
    public static void AddClient(Client client)
    {
        clients.Add(client);
    }

    // მომხმარებლის მიღების მეთოდი ID-ით, ენკაფსულაციის დემონსტრაციისთვის
    public static Client GetClient(Guid clientId)
    {
        return clients.FirstOrDefault(c => c.Id == clientId);
    }

    // ყველა მომხმარებლის მიღების მეთოდი, ენკაფსულაციის დემონსტრაციისთვის
    public static List<Client> GetAllClients()
    {
        return clients;
    }

    // მომხმარებლის წაშლის მეთოდი ID-ით, ენკაფსულაციის დემონსტრაციისთვის
    public static void RemoveClient(Guid clientId)
    {
        var client = GetClient(clientId);
        if (client != null)
        {
            clients.Remove(client);
        }
    }
}

// Comparer კლასი დეპოზიტებისთვის დახარისხების გაადვილების მიზნით, პოლიმორფიზმის დემონსტრაციისთვის
public class DepositComparer : IComparer<Deposit>
{
    public int Compare(Deposit x, Deposit y)
    {
        return y.Amount.CompareTo(x.Amount);
    }
}

// Comparer კლასი სესხებისთვის დახარისხების გაადვილების მიზნით, პოლიმორფიზმის დემონსტრაციისთვის
public class LoanComparer : IComparer<Loan>
{
    public int Compare(Loan x, Loan y)
    {
        return x.TermInMonths.CompareTo(y.TermInMonths);
    }
}

// სტატიკური კლასი სხვადასხვა საბანკო ოპერაციებისთვის, ენკაფსულაციის დემონსტრაციისთვის
public static class BankingOperations
{
    // დეპოზიტების დახარისხების მეთოდი DepositComparer-ის გამოყენებით, ენკაფსულაციის დემონსტრაციისთვის
    public static void SortDeposits(List<Deposit> deposits)
    {
        deposits.Sort(new DepositComparer());
    }

    // სესხების დახარისხების მეთოდი LoanComparer-ის გამოყენებით, ენკაფსულაციის დემონსტრაციისთვის
    public static void SortLoans(List<Loan> loans)
    {
        loans.Sort(new LoanComparer());
    }

    // ყველაზე მაღალი დავალიანების მქონე კორპორატიული მომხმარებლების მიღების მეთოდი, ენკაფსულაციისა და LINQ გამოყენებით
    public static List<CorporateClient> GetTopDebtCorporateClients(int topM)
    {
        return ClientDirectory.GetAllClients()
            .OfType<CorporateClient>()
            .OrderByDescending(c => c.Loans.Sum(l => l.RemainingAmount))
            .Take(topM)
            .ToList();
    }

    // წლიური სესხების ანგარიშის მიღების მეთოდი, ენკაფსულაციისა და LINQ გამოყენებით
    public static decimal GetAnnualLoanReport(int year, out decimal totalIndividualLoans, out decimal totalCorporateLoans)
    {
        totalIndividualLoans = ClientDirectory.GetAllClients()
            .OfType<IndividualClient>()
            .Sum(c => c.Loans.Where(l => l.LoanDate.Year == year).Sum(l => l.Amount));

        totalCorporateLoans = ClientDirectory.GetAllClients()
            .OfType<CorporateClient>()
            .Sum(c => c.Loans.Where(l => l.LoanDate.Year == year).Sum(l => l.Amount));

        return totalIndividualLoans + totalCorporateLoans;
    }
}
