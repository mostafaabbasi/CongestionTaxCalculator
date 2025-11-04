
using CongestionTaxCalculator;

var calculator = new Calculator();

var car = new Car();
var dates = new[]
{
    DateTime.Parse("2013-02-08 06:27:00"),
    DateTime.Parse("2013-02-08 06:20:27"),
    DateTime.Parse("2013-02-08 14:35:00"),
    DateTime.Parse("2013-02-08 15:29:00"),
    DateTime.Parse("2013-02-08 15:47:00"),
    DateTime.Parse("2013-02-08 16:01:00")
};

var tax = calculator.GetTax(car, dates);
Console.WriteLine($"Total Tax: {tax} SEK");