using System.ComponentModel.DataAnnotations;

namespace MVCApp.Models;

public class FormNumberInputModel
{
    public float CurrentNumber { get; set; }
    public string? SortType { get; set; }
}