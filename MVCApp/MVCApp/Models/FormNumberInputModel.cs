using System.ComponentModel.DataAnnotations;

namespace MVCApp.Models;

public class FormNumberInputModel
{
    [RegularExpression(@"^\d+[0-9]*$", ErrorMessage = "Please enter a whole number")]
    public int CurrentNumber { get; set; }

    public string? SortType { get; set; }
}