using System.ComponentModel.DataAnnotations;

namespace BuilderERP.Web.Models;

public class ResetPasswordViewModel
{
    public Guid UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Display(Name = "Confirm New Password")]
    [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
