using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace p48_lessons_asp.Models;

public class CreateProductViewModel
{
    [Required(ErrorMessage = "Enter product title")]
    [StringLength(100, ErrorMessage ="Title must have 100 symbols maximum")]
    [Display(Name="Product title")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter price")]
    [Range(1, 10000000, ErrorMessage ="Price must be greater then zero")]
    [Display(Name="Price (₴)")]
    public double Price { get; set; }

    [Required(ErrorMessage ="Select category")]
    [Display(Name="Category")]
    public Guid CategoryId { get; set; }

    // Властивість для заповнення випадаючого списку на сторінці
    public SelectList? Categories{get;set;}
}
