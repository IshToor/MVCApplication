using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MVCApp.Models;
public class SortedDataModel
{
    [Key]
    public int Id { get; set; }
    
    public string? SortedNumberSequence { get; set; }

    public string? SortType { get; set; }

    public string? TimeElapsed { get; set; }
}